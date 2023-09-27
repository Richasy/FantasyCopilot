// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.App;

/// <summary>
/// Fast translation requests.
/// </summary>
public sealed class QuickTranslateRequest
{
    /// <summary>
    /// The content to translate.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Source language.
    /// </summary>
    public string TargetLanguage { get; set; }

    /// <summary>
    /// Text segment index.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// plain or html.
    /// </summary>
    public string Type { get; set; }
}
