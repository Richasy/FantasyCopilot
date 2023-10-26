// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel.AI.Embeddings;
using RichasyAssistant.Models.App.Connectors;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.CustomConnector;

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
    public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IList<string> data, CancellationToken cancellationToken = default)
    {
        Utils.VerifyNotNull(data);
        var result = new List<ReadOnlyMemory<float>>();
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
            result.Add(new ReadOnlyMemory<float>(embeddingResult.Embedding.ToArray()));
        }

        return result;
    }

    internal class EmbeddingResult
    {
        [JsonPropertyName("embedding")]
        public List<float> Embedding { get; set; }
    }
}
