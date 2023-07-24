// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Services.Interfaces;

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
    /// <returns><see cref="Task"/>.</returns>
    Task ReloadConfigAsync();
}
