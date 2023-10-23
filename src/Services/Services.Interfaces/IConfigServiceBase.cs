// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Services.Interfaces;

/// <summary>
/// A service that has a configuration.
/// </summary>
public interface IConfigServiceBase
{
    /// <summary>
    /// Does the current user have a valid config.
    /// </summary>
    bool HasValidConfig { get; }

    /// <summary>
    /// Reload config.
    /// </summary>
    void ReloadConfig();
}
