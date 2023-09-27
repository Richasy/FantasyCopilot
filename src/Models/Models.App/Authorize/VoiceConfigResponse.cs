// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Text.Json.Serialization;
using static FantasyCopilot.Models.App.AppConfig;

namespace FantasyCopilot.Models.App.Authorize;

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
