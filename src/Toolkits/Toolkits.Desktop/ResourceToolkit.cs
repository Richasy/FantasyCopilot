// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;
using Windows.ApplicationModel.Resources.Core;

namespace RichasyAssistant.Toolkits;

/// <summary>
/// Resource toolkit.
/// </summary>
public sealed class ResourceToolkit : IResourceToolkit
{
    /// <inheritdoc/>
    public string GetLocalizedString(StringNames resourceName)
        => GetLocalizedString(resourceName.ToString());

    /// <inheritdoc/>
    public string GetLocalizedString(string resourceName)
        => ResourceManager.Current.MainResourceMap[$"Resources/{resourceName}"].Candidates[0].ValueAsString;
}
