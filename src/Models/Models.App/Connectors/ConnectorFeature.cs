// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Text.Json.Serialization;

namespace FantasyCopilot.Models.App.Connectors;

/// <summary>
/// Connector feature.
/// </summary>
public sealed class ConnectorFeature
{
    /// <summary>
    /// Feature type.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// Endpoint list.
    /// </summary>
    [JsonPropertyName("endpoints")]
    public List<ConnectorEndpoint> Endpoints { get; set; }
}
