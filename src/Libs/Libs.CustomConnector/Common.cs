// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json.Serialization;

namespace RichasyAssistant.Libs.CustomConnector;

internal class MessageResult
{
    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("isError")]
    public bool IsError { get; set; }
}

internal class RequestBase
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("settings")]
    public RequestSettings Settings { get; set; }
}

internal class RequestSettings
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("maxResponseTokens")]
    public int MaxResponseTokens { get; set; }

    [JsonPropertyName("topP")]
    public double TopP { get; set; }

    [JsonPropertyName("frequencyPenalty")]
    public double FrequencyPenalty { get; set; }

    [JsonPropertyName("presencePenalty")]
    public double PresencePenalty { get; set; }
}
