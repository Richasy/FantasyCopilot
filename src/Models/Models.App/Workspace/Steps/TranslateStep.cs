// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.App.Workspace.Steps;

/// <summary>
/// Translate step.
/// </summary>
public sealed class TranslateStep
{
    /// <summary>
    /// Source language.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Target language.
    /// </summary>
    public string Target { get; set; }
}
