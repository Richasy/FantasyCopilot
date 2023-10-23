// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json.Serialization;

namespace RichasyAssistant.Models.App.Connectors;

/// <summary>
/// Connector configuration.
/// </summary>
public sealed class ConnectorConfig
{
    /// <summary>
    /// Connector id.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// Display name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// API access path, combining with endpoint.
    /// </summary>
    [JsonPropertyName("base_url")]
    public string BaseUrl { get; set; }

    /// <summary>
    /// If the API requires special configuration,
    /// please provide the relative path of the configuration file.
    /// </summary>
    [JsonPropertyName("config_path")]
    public string ConfigPath { get; set; }

    /// <summary>
    /// Connector description file, which will automatically open during import.
    /// </summary>
    [JsonPropertyName("readme")]
    public string ReadMe { get; set; }

    /// <summary>
    /// The relative path to the program execution file.
    /// </summary>
    [JsonPropertyName("execute_name")]
    public string ExecuteName { get; set; }

    /// <summary>
    /// Script file at initialization.
    /// </summary>
    [JsonPropertyName("script_file")]
    public string ScriptFile { get; set; }

    /// <summary>
    /// Script command at initialization.
    /// </summary>
    [JsonPropertyName("script_command")]
    public string ScriptCommand { get; set; }

    /// <summary>
    /// A list of features supported by the connector.
    /// </summary>
    [JsonPropertyName("features")]
    public List<ConnectorFeature> Features { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ConnectorConfig config && Id == config.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
