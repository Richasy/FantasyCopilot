// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Text.Json.Serialization;
using static FantasyCopilot.Models.App.AppConfig;

namespace FantasyCopilot.Models.App.Authorize;

/// <summary>
/// Translate config response.
/// </summary>
public sealed class TranslateConfigResponse
{
    /// <summary>
    /// Azure Translate config.
    /// </summary>
    [JsonPropertyName("azureTranslate")]
    public RegionConfig AzureTranslate { get; set; }

    /// <summary>
    /// Baidu Translate config.
    /// </summary>
    [JsonPropertyName("baiduTranslate")]
    public BaiduTranslateConfig BaiduTranslate { get; set; }

    /// <summary>
    /// Preferred translate, with options being <c>azure</c> and <c>baidu</c>.
    /// </summary>
    [JsonPropertyName("preferredTranslate")]
    public string PreferredTranslate { get; set; }
}
