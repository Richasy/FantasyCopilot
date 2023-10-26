// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Models.App.Workspace;

/// <summary>
/// Workflow step item.
/// </summary>
public sealed class WorkflowStep
{
    /// <summary>
    /// Skill type.
    /// </summary>
    public SkillType Skill { get; set; }

    /// <summary>
    /// Detailed definition of the skill (needs to be converted into JSON).
    /// </summary>
    public string Detail { get; set; }

    /// <summary>
    /// Step index.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Plugin command id.
    /// </summary>
    public string PluginCommandId { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is WorkflowStep step && Index == step.Index;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Index);
}
