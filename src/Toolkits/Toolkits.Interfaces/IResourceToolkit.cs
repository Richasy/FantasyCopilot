// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Toolkits.Interfaces;

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
