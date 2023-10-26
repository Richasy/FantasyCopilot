// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Models.App;

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
