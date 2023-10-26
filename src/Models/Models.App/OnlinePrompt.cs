// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App;

/// <summary>
/// Online prompt.
/// </summary>
public sealed class OnlinePrompt
{
    /// <summary>
    /// Prompt title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Prompt description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Prompt detail.
    /// </summary>
    public string Prompt { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is OnlinePrompt prompt && Title == prompt.Title;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Title);
}
