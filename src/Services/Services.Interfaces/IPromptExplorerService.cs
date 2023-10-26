// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Services.Interfaces;

/// <summary>
/// Interface definition of prompt explorer service.
/// </summary>
public interface IPromptExplorerService
{
    /// <summary>
    /// Get supported data sources.
    /// </summary>
    /// <returns>Prompt source.</returns>
    IEnumerable<OnlinePromptSource> GetSupportSources();

    /// <summary>
    /// Reload data source prompt list.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task<bool> ReloadSourceAsync(OnlinePromptSource source);

    /// <summary>
    /// Get details of the data source.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <returns><see cref="OnlinePromptList"/>.</returns>
    Task<OnlinePromptList> GetSourceDetailAsync(OnlinePromptSource source);
}
