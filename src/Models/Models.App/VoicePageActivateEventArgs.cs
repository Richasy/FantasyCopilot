// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.App;

/// <summary>
/// 音频页面激活事件参数.
/// </summary>
public sealed class VoicePageActivateEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VoicePageActivateEventArgs"/> class.
    /// </summary>
    public VoicePageActivateEventArgs(bool isTTS, string content)
    {
        IsTextToSpeech = isTTS;
        Content = content;
    }

    /// <summary>
    /// 是否为文本转语音.
    /// </summary>
    public bool IsTextToSpeech { get; set; }

    /// <summary>
    /// 文本内容或音频文件路径.
    /// </summary>
    public string Content { get; set; }
}
