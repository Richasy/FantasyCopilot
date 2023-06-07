// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.Models.App;

/// <summary>
/// Navigation event arguments.
/// </summary>
public sealed class NavigateEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigateEventArgs"/> class.
    /// </summary>
    /// <param name="type">Page id.</param>
    /// <param name="parameter">Navigation parameter.</param>
    public NavigateEventArgs(PageType type, object parameter)
    {
        Type = type;
        Parameter = parameter;
    }

    /// <summary>
    /// Page id.
    /// </summary>
    public PageType Type { get; set; }

    /// <summary>
    /// Navigation parameter.
    /// </summary>
    public object Parameter { get; set; }
}
