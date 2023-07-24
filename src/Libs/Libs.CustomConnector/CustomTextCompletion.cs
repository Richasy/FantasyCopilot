// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Net.Http.Json;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using FantasyCopilot.Models.App.Connectors;
using FantasyCopilot.Models.Constants;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Orchestration;

namespace FantasyCopilot.Libs.CustomConnector;

/// <summary>
/// Custom text completion service.
/// </summary>
public sealed class CustomTextCompletion : ITextCompletion
{
    private readonly ConnectorConfig _connectorConfig;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomTextCompletion"/> class.
    /// </summary>
    /// <param name="config">Connector config.</param>
    public CustomTextCompletion(ConnectorConfig config)
    {
        _connectorConfig = config;
        _httpClient = new HttpClient();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ITextResult>> GetCompletionsAsync(string text, CompleteRequestSettings requestSettings, CancellationToken cancellationToken = default)
    {
        Utils.VerifyNotNull(requestSettings);
        requestSettings ??= new();
        var config = _connectorConfig.Features
            .First(p => p.Type == ConnectorConstants.TextCompletionType)
            .Endpoints
            .First(p => p.Type == ConnectorConstants.TextCompletionRestType);
        var url = new Uri(_connectorConfig.BaseUrl + config.Path);
        var requestData = GetRequest(text, requestSettings);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(requestData),
        };
        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        var resultContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<MessageResult>(resultContent);
        if (!result.IsError && string.IsNullOrEmpty(result.Content))
        {
            result.IsError = true;
            result.Content = "Empty response";
        }

        return result.IsError
            ? throw new AIException(AIException.ErrorCodes.ServiceError, result.Content ?? "Something error")
            : new List<ITextResult> { new CustomTextResult(result) }.AsReadOnly();
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<ITextStreamingResult> GetStreamingCompletionsAsync(string text, CompleteRequestSettings requestSettings, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Utils.VerifyNotNull(requestSettings);
        requestSettings ??= new();
        var config = _connectorConfig.Features
            .First(p => p.Type == ConnectorConstants.TextCompletionType)
            .Endpoints
            .First(p => p.Type == ConnectorConstants.TextCompletionStreamType);
        var url = new Uri(_connectorConfig.BaseUrl + config.Path);
        var requestData = GetRequest(text, requestSettings);
        var json = JsonSerializer.Serialize(requestData);
        using var tcpClient = new TcpClient();
        tcpClient.Connect(url.Host, url.Port);
        var stream = tcpClient.GetStream();
        var contentLength = Encoding.UTF8.GetByteCount(json);
        var request = $"POST {url.PathAndQuery} HTTP/1.1\r\n" +
                 $"Host: {_connectorConfig.BaseUrl}\r\n" +
                 "Content-Type: application/json\r\n" +
                 $"Content-Length: {contentLength}\r\n" +
                 "\r\n" +
                 json;
        var requestBytes = Encoding.ASCII.GetBytes(request);
        await stream.WriteAsync(requestBytes, 0, requestBytes.Length, cancellationToken);
        yield return new CustomTextStreamingResult(stream, cancellationToken, () =>
        {
            // Try call stream cancel endpoint
            var cancelConfig = _connectorConfig.Features
            .First(p => p.Type == ConnectorConstants.TextCompletionType)
            .Endpoints
            .FirstOrDefault(p => p.Type == ConnectorConstants.TextCompletionStreamCancelType);
            if (cancelConfig != null)
            {
                var cancelUrl = new Uri(_connectorConfig.BaseUrl + cancelConfig.Path);
                var cancelRequest = new HttpRequestMessage(HttpMethod.Post, cancelUrl);
                _ = _httpClient.SendAsync(cancelRequest, CancellationToken.None);
            }
        });
    }

    private static RequestBase GetRequest(string text, CompleteRequestSettings requestSettings)
    {
        var settings = new RequestSettings
        {
            FrequencyPenalty = requestSettings.FrequencyPenalty,
            MaxResponseTokens = requestSettings.MaxTokens ?? AppConstants.DefaultMaxResponseTokens,
            PresencePenalty = requestSettings.PresencePenalty,
            Temperature = requestSettings.Temperature,
            TopP = requestSettings.TopP,
        };

        return new RequestBase
        {
            Message = text,
            Settings = settings,
        };
    }

    internal sealed class CustomTextResult : ITextResult
    {
        private readonly MessageResult _messageResult;

        public CustomTextResult(MessageResult resultData)
        {
            _messageResult = resultData;
            ModelResult = new ModelResult(resultData);
        }

        public ModelResult ModelResult { get; }

        public Task<string> GetCompletionAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_messageResult.Content);
    }

    internal sealed class CustomTextStreamingResult : ITextStreamingResult
    {
        private readonly Stream _stream;
        private readonly StreamReader _reader;
        private readonly CancellationToken _token;
        private readonly Action _action;
        private string _line;

        public CustomTextStreamingResult(Stream stream, CancellationToken token, Action cancelAction)
        {
            _stream = stream;
            _line = string.Empty;
            _reader = new StreamReader(stream);
            _token = token;
            _action = cancelAction;
            ModelResult = new ModelResult(stream);
        }

        public ModelResult ModelResult { get; }

        public Task<string> GetCompletionAsync(CancellationToken cancellationToken = default)
        {
            var msg = JsonSerializer.Deserialize<MessageResult>(_line);
            return Task.FromResult(msg.Content);
        }

        public async IAsyncEnumerable<string> GetCompletionStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using (_stream)
            using (_reader)
            {
                while ((_line = await _reader.ReadLineAsync(cancellationToken)) != null && _line != "[DONE]")
                {
                    try
                    {
                        _token.ThrowIfCancellationRequested();
                    }
                    catch (OperationCanceledException)
                    {
                        _action?.Invoke();
                        throw;
                    }

                    if (_line.StartsWith("error:"))
                    {
                        break;
                        throw new AIException(AIException.ErrorCodes.ServiceError, _line[6..].Trim());
                    }
                    else if (_line.StartsWith("{"))
                    {
                        yield return await GetCompletionAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
