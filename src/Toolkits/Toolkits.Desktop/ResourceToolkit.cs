// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using Windows.ApplicationModel.Resources.Core;

namespace FantasyCopilot.Toolkits;

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
