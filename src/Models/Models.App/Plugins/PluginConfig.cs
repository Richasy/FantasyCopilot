// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json.Serialization;

namespace RichasyAssistant.Models.App.Plugins;

/// <summary>
/// Plugin config.
/// </summary>
public class PluginConfig
{
    /// <summary>
    /// The currently schema version.
    /// </summary>
    [JsonPropertyName("schema_version")]
    public int SchemaVersion { get; set; }

    /// <summary>
    /// The name of the package as it appears on the UI.
    /// </summary>
    [JsonPropertyName("package_name")]
    public string Name { get; set; }

    /// <summary>
    /// A description of the package.
    /// </summary>
    [JsonPropertyName("package_desc")]
    public string Description { get; set; }

    /// <summary>
    /// The identifier of the package, which is also the name of the extracted folder.
    /// </summary>
    [JsonPropertyName("package_id")]
    public string Id { get; set; }

    /// <summary>
    /// The version of the current package.
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; }

    /// <summary>
    /// The author name of the package.
    /// </summary>
    [JsonPropertyName("author")]
    public string Author { get; set; }

    /// <summary>
    /// The author's profile.
    /// </summary>
    [JsonPropertyName("author_site")]
    public string AuthorSite { get; set; }

    /// <summary>
    /// The repository corresponding to this package.
    /// </summary>
    [JsonPropertyName("repository")]
    public string Repository { get; set; }

    /// <summary>
    /// The icon path for the package.
    /// </summary>
    [JsonPropertyName("logo_url")]
    public string LogoUrl { get; set; }

    /// <summary>
    /// A list of commands included in this package.
    /// </summary>
    [JsonPropertyName("commands")]
    public List<PluginCommand> Commands { get; set; }

    /// <summary>
    /// Text translation resources.
    /// </summary>
    [JsonPropertyName("resources")]
    public Dictionary<string, Dictionary<string, string>> Resources { get; set; }
}
