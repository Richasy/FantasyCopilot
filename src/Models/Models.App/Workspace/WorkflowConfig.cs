// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.App.Workspace;

/// <summary>
/// Workflow config.
/// </summary>
public sealed class WorkflowConfig
{
    /// <summary>
    /// Identifier.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Input method.
    /// </summary>
    public WorkflowStep Input { get; set; }

    /// <summary>
    /// Output method.
    /// </summary>
    public WorkflowStep Output { get; set; }

    /// <summary>
    /// Follwing steps.
    /// </summary>
    public List<WorkflowStep> Steps { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is WorkflowConfig config && Id == config.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
