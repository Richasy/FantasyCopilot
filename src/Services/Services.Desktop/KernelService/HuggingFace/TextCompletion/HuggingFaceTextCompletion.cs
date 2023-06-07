// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.TextCompletion;

namespace FantasyCopilot.Services;

/// <summary>
/// HuggingFace text completion service.
/// </summary>
public sealed class HuggingFaceTextCompletion : ITextCompletion, IDisposable
{
    private const string HttpUserAgent = "Microsoft-Semantic-Kernel";
    private const string HuggingFaceApiEndpoint = "https://api-inference.huggingface.co/models";

    private readonly string _model;
    private readonly Uri _endpoint;
    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient = true;
    private readonly string _apiKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="HuggingFaceTextCompletion"/> class.
    /// Using HuggingFace API for service call, see https://huggingface.co/docs/api-inference/index.
    /// </summary>
    /// <param name="model">Model to use for service API call.</param>
    /// <param name="endpoint">Endpoint for service API call.</param>
    /// <param name="apiKey">HuggingFace API key, see https://huggingface.co/docs/api-inference/quicktour#running-inference-with-api-requests.</param>
    public HuggingFaceTextCompletion(string model, string endpoint = null, string apiKey = null, HttpClient client = null)
    {
        _model = model;
        _endpoint = string.IsNullOrEmpty(endpoint) ? null : new Uri(endpoint);
        _apiKey = apiKey;

        _httpClient = client ?? new HttpClient(new HttpClientHandler { CheckCertificateRevocationList = true }, disposeHandler: false);
        _disposeHttpClient = false; // Disposal is unnecessary as we either use a non-disposable handler or utilize a custom HTTP client that we should not dispose.
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<ITextCompletionStreamingResult> GetStreamingCompletionsAsync(
        string text,
        CompleteRequestSettings requestSettings,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var completion in await ExecuteGetCompletionsAsync(text, cancellationToken).ConfigureAwait(false))
        {
            yield return completion;
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ITextCompletionResult>> GetCompletionsAsync(
        string text,
        CompleteRequestSettings requestSettings,
        CancellationToken cancellationToken = default)
        => await ExecuteGetCompletionsAsync(text, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    private async Task<IReadOnlyList<ITextCompletionStreamingResult>> ExecuteGetCompletionsAsync(string text, CancellationToken cancellationToken = default)
    {
        try
        {
            var completionRequest = new TextCompletionRequest
            {
                Input = text,
            };

            using var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = GetRequestUri(),
                Content = new StringContent(JsonSerializer.Serialize(completionRequest)),
            };

            httpRequestMessage.Headers.Add("User-Agent", HttpUserAgent);
            if (!string.IsNullOrEmpty(_apiKey))
            {
                httpRequestMessage.Headers.Add("Authorization", $"Bearer {_apiKey}");
            }

            using var response = await _httpClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            var completionResponse = JsonSerializer.Deserialize<List<TextCompletionResponse>>(body);

            return completionResponse is null
                ? throw new AIException(AIException.ErrorCodes.InvalidResponseContent, "Unexpected response from model")
                {
                    Data = { { "ModelResponse", body } },
                }
                : (IReadOnlyList<ITextCompletionStreamingResult>)completionResponse.ConvertAll(c => new TextCompletionStreamingResult(c.Text));
        }
        catch (Exception e) when (e is not AIException)
        {
            throw new AIException(
                AIException.ErrorCodes.UnknownError,
                $"Something went wrong: {e.Message}",
                e);
        }
    }

    /// <summary>
    /// Retrieves the request URI based on the provided endpoint and model information.
    /// </summary>
    /// <returns>
    /// A <see cref="Uri"/> object representing the request URI.
    /// </returns>
    private Uri GetRequestUri()
    {
        var baseUrl = HuggingFaceApiEndpoint;

        if (_endpoint?.AbsoluteUri != null)
        {
            baseUrl = _endpoint!.AbsoluteUri;
        }
        else if (_httpClient.BaseAddress?.AbsoluteUri != null)
        {
            baseUrl = _httpClient.BaseAddress!.AbsoluteUri;
        }

        return new Uri($"{baseUrl!.TrimEnd('/')}/{_model}");
    }
}
