// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Text.Json.Serialization;

namespace FantasyCopilot.Models.App.Connectors;

/// <summary>
/// Connector endpoint.
/// </summary>
public sealed class ConnectorEndpoint
{
    /// <summary>
    /// Endpoint type.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// Endpoint path, combined with BaseUrl, is a complete API.
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; }
}
