// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Models.App.Files;

/// <summary>
/// Audio search selection type.
/// </summary>
public sealed class AudioSearchEntry
{
    /// <summary>
    /// Search type.
    /// </summary>
    public AudioSearchType Type { get; set; }

    /// <summary>
    /// Type name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Type icon.
    /// </summary>
    public FluentSymbol Icon { get; set; }

    /// <summary>
    /// Placeholder text.
    /// </summary>
    public string PlaceholderText { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is AudioSearchEntry entry && Type == entry.Type;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Type);
}
