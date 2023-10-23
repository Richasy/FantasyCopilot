// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Models.App;

/// <summary>
/// App navigation item.
/// </summary>
public sealed class NavigateItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigateItem"/> class.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="type">Page identification.</param>
    /// <param name="symbol">Icon.</param>
    public NavigateItem(string title, PageType type, FluentSymbol symbol)
    {
        Title = title;
        Type = type;
        Symbol = symbol;
    }

    /// <summary>
    /// Page title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Page identifier.
    /// </summary>
    public PageType Type { get; }

    /// <summary>
    /// Page icon.
    /// </summary>
    public FluentSymbol Symbol { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is NavigateItem item && Symbol == item.Symbol;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Symbol);
}
