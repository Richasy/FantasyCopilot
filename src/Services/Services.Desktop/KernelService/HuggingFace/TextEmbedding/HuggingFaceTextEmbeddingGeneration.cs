// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.Embeddings;

namespace FantasyCopilot.Services;

/// <summary>
/// HuggingFace embedding generation service.
/// </summary>
public sealed class HuggingFaceTextEmbeddingGeneration : ITextEmbeddingGeneration, IDisposable
{
    private const string HttpUserAgent = "Microsoft-Semantic-Kernel";

    private readonly string _model;
    private readonly string _key;
    private readonly Uri _endpoint;
    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="HuggingFaceTextEmbeddingGeneration"/> class.
    /// Using default <see cref="HttpClientHandler"/> implementation.
    /// </summary>
    /// <param name="endpoint">Endpoint for service API call.</param>
    /// <param name="model">Model to use for service API call.</param>
    /// <param name="apiKey">Api Key.</param>
    public HuggingFaceTextEmbeddingGeneration(Uri endpoint, string model, string apiKey, HttpClient httpClient)
    {
        _endpoint = endpoint;
        _model = model;
        _key = apiKey;

        _httpClient = httpClient;
        _disposeHttpClient = false; // Disposal is unnecessary as we either use a non-disposable handler or utilize a custom HTTP client that we should not dispose.
    }

    /// <inheritdoc/>
    public async Task<IList<Embedding<float>>> GenerateEmbeddingsAsync(IList<string> data, CancellationToken cancellationToken = default)
        => await ExecuteEmbeddingRequestAsync(data, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    /// <summary>
    /// Performs HTTP request to given endpoint for embedding generation.
    /// </summary>
    /// <param name="data">Data to embed.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>List of generated embeddings.</returns>
    /// <exception cref="AIException">Exception when backend didn't respond with generated embeddings.</exception>
    private async Task<IList<Embedding<float>>> ExecuteEmbeddingRequestAsync(IList<string> data, CancellationToken cancellationToken)
    {
        try
        {
            var embeddingRequest = new TextEmbeddingRequest
            {
                Input = data,
            };

            using var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = GetRequestUri(),
                Content = new StringContent(JsonSerializer.Serialize(embeddingRequest)),
            };

            httpRequestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _key);
            httpRequestMessage.Headers.Add("User-Agent", HttpUserAgent);

            var response = await _httpClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var embeddingResponse = JsonSerializer.Deserialize<TextEmbeddingResponse>(body);

            return embeddingResponse?.Embeddings?.Select(l => new Embedding<float>(l.Embedding!, transferOwnership: true)).ToList();
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
        string baseUrl = null;

        if (_endpoint?.AbsoluteUri != null)
        {
            baseUrl = _endpoint!.AbsoluteUri;
        }
        else if (_httpClient.BaseAddress?.AbsoluteUri != null)
        {
            baseUrl = _httpClient.BaseAddress!.AbsoluteUri;
        }
        else
        {
            throw new AIException(AIException.ErrorCodes.InvalidConfiguration, "No endpoint or HTTP client base address has been provided");
        }

        return new Uri($"{baseUrl!.TrimEnd('/')}/{_model}");
    }
}
