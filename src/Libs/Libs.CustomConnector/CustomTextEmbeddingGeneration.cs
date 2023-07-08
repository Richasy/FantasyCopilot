// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FantasyCopilot.Models.App.Connectors;
using FantasyCopilot.Models.Constants;
using Microsoft.SemanticKernel.AI.Embeddings;

namespace FantasyCopilot.Libs.CustomConnector;

/// <summary>
/// Custom text embedding generation service.
/// </summary>
public sealed class CustomTextEmbeddingGeneration : ITextEmbeddingGeneration
{
    private readonly ConnectorConfig _connectorConfig;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomTextEmbeddingGeneration"/> class.
    /// </summary>
    /// <param name="config">Connector config.</param>
    public CustomTextEmbeddingGeneration(ConnectorConfig config)
    {
        _connectorConfig = config;
        _httpClient = new HttpClient();
    }

    /// <inheritdoc/>
    public async Task<IList<Embedding<float>>> GenerateEmbeddingsAsync(IList<string> data, CancellationToken cancellationToken = default)
    {
        Utils.VerifyNotNull(data);
        var result = new List<Embedding<float>>();
        var config = _connectorConfig.Features
            .First(p => p.Type == ConnectorConstants.EmbeddingType)
            .Endpoints
            .First(p => p.Type == ConnectorConstants.EmbeddingRestType);
        var url = new Uri(_connectorConfig.BaseUrl + config.Path);
        foreach (var text in data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(text, Encoding.UTF8, "text/plain");
            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var embeddingResult = JsonSerializer.Deserialize<EmbeddingResult>(content);
            result.Add(new Embedding<float>(embeddingResult.Embedding, transferOwnership: true));
        }

        return result;
    }

    internal class EmbeddingResult
    {
        [JsonPropertyName("embedding")]
        public List<float> Embedding { get; set; }
    }
}
