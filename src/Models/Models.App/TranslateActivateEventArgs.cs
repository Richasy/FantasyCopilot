// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App;

/// <summary>
/// Translate activate event args.
/// </summary>
public sealed class TranslateActivateEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateActivateEventArgs"/> class.
    /// </summary>
    public TranslateActivateEventArgs(
        string content,
        string targetLanguage = default)
    {
        Content = content;
        TargetLanguage = targetLanguage;
    }

    /// <summary>
    /// What needs to be translated.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Target language.
    /// </summary>
    public string TargetLanguage { get; set; }
}
