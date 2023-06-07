// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.App.Gpt;

/// <summary>
/// Session metadata.
/// </summary>
public sealed class SessionMetadata : IdentityModelBase
{
    /// <summary>
    /// System prompt.
    /// </summary>
    public string SystemPrompt { get; set; }

    /// <summary>
    /// Clone object.
    /// </summary>
    /// <returns><see cref="SessionMetadata"/>.</returns>
    public SessionMetadata Clone()
    => new()
    {
        Id = Id,
        Name = Name,
        Description = Description,
        SystemPrompt = SystemPrompt,
    };
}
