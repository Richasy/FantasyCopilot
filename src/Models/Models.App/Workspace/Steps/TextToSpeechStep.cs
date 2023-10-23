// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App.Workspace.Steps;

/// <summary>
/// Text to speech step.
/// </summary>
public sealed class TextToSpeechStep
{
    /// <summary>
    /// Selected language.
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Selected voice.
    /// </summary>
    public string Voice { get; set; }
}
