// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.Image;

namespace RichasyAssistant.Models.App.Workspace;

/// <summary>
/// Image skill configuration.
/// </summary>
public sealed class ImageSkillConfig : GenerateOptions
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
    /// Negative prompt.
    /// </summary>
    public string NegativePrompt { get; set; }

    /// <summary>
    /// Skill id.
    /// </summary>
    public string Id { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ImageSkillConfig config && Id == config.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
