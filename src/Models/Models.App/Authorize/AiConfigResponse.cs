// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json.Serialization;
using static RichasyAssistant.Models.App.AppConfig;

namespace RichasyAssistant.Models.App.Authorize;

/// <summary>
/// AI config response.
/// </summary>
public sealed class AiConfigResponse
{
    /// <summary>
    /// Azure Open AI config.
    /// </summary>
    [JsonPropertyName("azureOpenAI")]
    public AzureOpenAIConfig AzureOpenAI { get; set; }

    /// <summary>
    /// Open AI config.
    /// </summary>
    [JsonPropertyName("openAI")]
    public OpenAIConfig OpenAI { get; set; }

    /// <summary>
    /// Preferred AI, with options being <c>azoai</c> and <c>oai</c>.
    /// </summary>
    [JsonPropertyName("preferredAI")]
    public string PreferredAI { get; set; }
}
