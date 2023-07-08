// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Net.Http.Json;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FantasyCopilot.Models.App.Connectors;
using FantasyCopilot.Models.Constants;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.ChatCompletion;

namespace FantasyCopilot.Libs.CustomConnector;

/// <summary>
/// Custom chat completion service.
/// </summary>
public sealed class CustomChatCompletion : IChatCompletion
{
    private readonly ConnectorConfig _connectorConfig;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomChatCompletion"/> class.
    /// </summary>
    /// <param name="config">Connector config.</param>
    public CustomChatCompletion(ConnectorConfig config)
    {
        _connectorConfig = config;
        _httpClient = new HttpClient();
    }

    /// <inheritdoc/>
    public ChatHistory CreateNewChat(string? instructions = null)
    {
        var chatHistory = new ChatHistory();
        if (!string.IsNullOrEmpty(instructions))
        {
            chatHistory.AddSystemMessage(instructions);
        }

        return chatHistory;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<IChatResult>> GetChatCompletionsAsync(ChatHistory chat, ChatRequestSettings? requestSettings = null, CancellationToken cancellationToken = default)
    {
        Utils.VerifyNotNull(chat);
        requestSettings ??= new();
        var config = _connectorConfig.Features
            .First(p => p.Type == ConnectorConstants.ChatType)
            .Endpoints
            .First(p => p.Type == ConnectorConstants.ChatRestType);
        var url = new Uri(_connectorConfig.BaseUrl + config.Path);
        var requestData = GetRequest(chat, requestSettings);
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
            : new List<IChatResult> { new CustomChatCompletionResult(result) }.AsReadOnly();
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<IChatStreamingResult> GetStreamingChatCompletionsAsync(ChatHistory chat, ChatRequestSettings? requestSettings = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Utils.VerifyNotNull(chat);
        requestSettings ??= new();
        var config = _connectorConfig.Features
            .First(p => p.Type == ConnectorConstants.ChatType)
            .Endpoints
            .First(p => p.Type == ConnectorConstants.ChatStreamType);
        var url = new Uri(_connectorConfig.BaseUrl + config.Path);
        var requestData = GetRequest(chat, requestSettings);
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
        yield return new CustomChatStreamingResult(stream, cancellationToken, () =>
        {
            // Try call stream cancel endpoint
            var cancelConfig = _connectorConfig.Features
            .First(p => p.Type == ConnectorConstants.ChatType)
            .Endpoints
            .FirstOrDefault(p => p.Type == ConnectorConstants.ChatStreamCancelType);
            if (cancelConfig != null)
            {
                var cancelUrl = new Uri(_connectorConfig.BaseUrl + cancelConfig.Path);
                var cancelRequest = new HttpRequestMessage(HttpMethod.Post, cancelUrl);
                _ = _httpClient.SendAsync(cancelRequest, CancellationToken.None);
            }
        });
    }

    private static Request GetRequest(ChatHistory chat, ChatRequestSettings requestSettings)
    {
        var userMsg = chat.LastOrDefault(p => p.Role == AuthorRole.User)
            ?? throw new AIException(AIException.ErrorCodes.InvalidRequest);
        var history = new List<Message>();
        for (var i = 0; i < chat.Messages.Count - 1; i++)
        {
            var item = chat.Messages[i];
            var msg = new Message { Content = item.Content, Role = item.Role.ToString() };
            history.Add(msg);
        }

        var settings = new RequestSettings
        {
            FrequencyPenalty = requestSettings.FrequencyPenalty,
            MaxResponseTokens = requestSettings.MaxTokens,
            PresencePenalty = requestSettings.PresencePenalty,
            Temperature = requestSettings.Temperature,
            TopP = requestSettings.TopP,
        };

        return new Request
        {
            Message = userMsg.Content,
            History = history,
            Settings = settings,
        };
    }

    internal class CustomChatCompletionResult : IChatResult
    {
        private readonly MessageResult _messageResult;

        public CustomChatCompletionResult(MessageResult result)
            => _messageResult = result;

        public Task<ChatMessageBase> GetChatMessageAsync(CancellationToken cancellationToken = default)
            => Task.FromResult((ChatMessageBase)new ChatMessage(AuthorRole.Assistant, _messageResult.Content));
    }

    internal class CustomChatStreamingResult : IChatStreamingResult
    {
        private readonly Stream _stream;
        private readonly StreamReader _reader;
        private readonly CancellationToken _token;
        private readonly Action _action;
        private string _line;

        public CustomChatStreamingResult(Stream stream, CancellationToken token, Action cancelAction)
        {
            _stream = stream;
            _line = string.Empty;
            _reader = new StreamReader(stream);
            _token = token;
            _action = cancelAction;
        }

        public Task<ChatMessageBase> GetChatMessageAsync(CancellationToken cancellationToken = default)
        {
            var msg = JsonSerializer.Deserialize<MessageResult>(_line);
            return Task.FromResult((ChatMessageBase)new ChatMessage(AuthorRole.Assistant, msg.Content));
        }

        public async IAsyncEnumerable<ChatMessageBase> GetStreamingChatMessageAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
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
                        yield return await GetChatMessageAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }
        }
    }

    internal class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    internal class Request : RequestBase
    {
        [JsonPropertyName("history")]
        public List<Message> History { get; set; }
    }

    private sealed class ChatMessage : ChatMessageBase
    {
        public ChatMessage(AuthorRole authorRole, string content)
            : base(authorRole, content)
        {
        }
    }
}
