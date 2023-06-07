// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Globalization;

namespace FantasyCopilot.Models.App;

/// <summary>
/// Locale information.
/// </summary>
public sealed class LocaleInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocaleInfo"/> class.
    /// </summary>
    public LocaleInfo(string id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocaleInfo"/> class.
    /// </summary>
    public LocaleInfo(CultureInfo info)
    {
        Id = info.Name;
        Name = info.DisplayName;
    }

    /// <summary>
    /// Language code.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Display name.
    /// </summary>
    public string Name { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is LocaleInfo info && Id == info.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
