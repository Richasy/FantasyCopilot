// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.Models.App.Files;

/// <summary>
/// File search selection type.
/// </summary>
public sealed class FileSearchEntry
{
    /// <summary>
    /// Search type.
    /// </summary>
    public FileSearchType Type { get; set; }

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
    public override bool Equals(object? obj) => obj is FileSearchEntry entry && Type == entry.Type;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Type);
}
