// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Text.Json.Serialization;

namespace FantasyCopilot.Models.App;

/// <summary>
/// 协议提示词.
/// </summary>
public sealed class ProtocolPrompt
{
    /// <summary>
    /// 显示在界面上的提示词.
    /// </summary>
    [JsonPropertyName("message")]
    public string DisplayMessage { get; set; }

    /// <summary>
    /// 实际发送给模型的提示词.
    /// </summary>
    [JsonPropertyName("prompt")]
    public string ActualMessage { get; set; }

    /// <summary>
    /// Temperature.
    /// </summary>
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    /// <summary>
    /// 最大响应令牌数.
    /// </summary>
    [JsonPropertyName("tokens")]
    public int MaxResponseTokens { get; set; }

    /// <summary>
    /// 是否使用对话模型，反之则使用文本完成模型.
    /// </summary>
    [JsonPropertyName("chat")]
    public bool UseChat { get; set; }
}
