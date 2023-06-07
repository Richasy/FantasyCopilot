// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.App.Workspace.Steps;

/// <summary>
/// Variable redirection step.
/// </summary>
public sealed class VariableRedirectStep
{
    /// <summary>
    /// Source variable name.
    /// </summary>
    public string SourceName { get; set; }

    /// <summary>
    /// The name of the target variable.
    /// </summary>
    public string TargetName { get; set; }
}
