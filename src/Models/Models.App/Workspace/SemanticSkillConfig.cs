// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.Gpt;

namespace RichasyAssistant.Models.App.Workspace;

/// <summary>
/// Semantic skill configuration.
/// </summary>
public sealed class SemanticSkillConfig : SessionOptions
{
    /// <summary>
    /// Function name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Function description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Prompt.
    /// </summary>
    public string Prompt { get; set; }

    /// <summary>
    /// Skill id.
    /// </summary>
    public string Id { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SemanticSkillConfig config && Id == config.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
