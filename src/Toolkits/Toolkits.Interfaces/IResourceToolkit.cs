// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.Toolkits.Interfaces;

/// <summary>
/// Interface of resource toolkit.
/// </summary>
public interface IResourceToolkit
{
    /// <summary>
    /// Get localized string.
    /// </summary>
    /// <param name="resourceName">Resource name.</param>
    /// <returns>Localized string.</returns>
    string GetLocalizedString(StringNames resourceName);

    /// <summary>
    /// Get localized string.
    /// </summary>
    /// <param name="resourceName">Resource name.</param>
    /// <returns>Localized string.</returns>
    string GetLocalizedString(string resourceName);
}
