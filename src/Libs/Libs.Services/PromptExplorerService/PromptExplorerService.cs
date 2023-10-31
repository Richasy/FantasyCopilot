// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Prompt explorer service.
/// </summary>
public sealed partial class PromptExplorerService : ServiceBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptExplorerService"/> class.
    /// </summary>
    private PromptExplorerService()
        => _cacheList = new Dictionary<OnlinePromptSource, OnlinePromptList>();

    /// <summary>
    /// Get supported data sources.
    /// </summary>
    /// <returns>Prompt source.</returns>
    public static List<OnlinePromptSource> GetSupportSources()
    {
        var list = new List<OnlinePromptSource>
        {
            OnlinePromptSource.FlowGptEnTrending,
            OnlinePromptSource.AwesomePrompt,
        };

        var language = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.DefaultContext.Languages.FirstOrDefault();
        if (language.StartsWith("zh-", StringComparison.OrdinalIgnoreCase))
        {
            list.Add(OnlinePromptSource.FlowGptZhTrending);
            list.Add(OnlinePromptSource.AwesomePromptZh);
        }

        return list;
    }

    /// <summary>
    /// Get details of the data source.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <returns><see cref="OnlinePromptList"/>.</returns>
    public async Task<OnlinePromptList> GetSourceDetailAsync(OnlinePromptSource source)
    {
        var hasCache = await TryLoadSourceAsync(source);
        return hasCache ? _cacheList[source] : default;
    }

    /// <summary>
    /// Reload data source prompt list.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task<bool> ReloadSourceAsync(OnlinePromptSource source)
    {
        try
        {
            var data = source switch
            {
                OnlinePromptSource.AwesomePromptZh => await GetAwesomePromptsZhAsync(),
                OnlinePromptSource.AwesomePrompt => await GetAwesomePromptsAsync(),
                OnlinePromptSource.FlowGptEnTrending => await GetFlowGptTrendingAsync(source),
                OnlinePromptSource.FlowGptZhTrending => await GetFlowGptTrendingAsync(source),
                _ => throw new NotSupportedException(),
            };

            await SaveCacheAsync(source, data);
            _cacheList[source] = data;
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"An error occurred while trying to get {source}");
            return false;
        }
    }
}
