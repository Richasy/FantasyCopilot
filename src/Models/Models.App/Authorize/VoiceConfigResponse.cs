// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json.Serialization;
using static RichasyAssistant.Models.App.AppConfig;

namespace RichasyAssistant.Models.App.Authorize;

/// <summary>
/// Voice config response.
/// </summary>
public sealed class VoiceConfigResponse
{
    /// <summary>
    /// Azure Voice config.
    /// </summary>
    [JsonPropertyName("azureVoice")]
    public RegionConfig AzureVoice { get; set; }
}
