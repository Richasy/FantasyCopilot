// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.App.Workspace.Steps;

/// <summary>
/// Create variable.
/// </summary>
public sealed class VariableCreateStep
{
    /// <summary>
    /// Variable name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Variable value.
    /// </summary>
    public string Value { get; set; }
}
