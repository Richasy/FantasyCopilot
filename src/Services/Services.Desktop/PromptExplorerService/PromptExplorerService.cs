// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using Microsoft.Extensions.Logging;

namespace FantasyCopilot.Services;

/// <summary>
/// Prompt explorer service.
/// </summary>
public sealed partial class PromptExplorerService : IPromptExplorerService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptExplorerService"/> class.
    /// </summary>
    public PromptExplorerService(
        IFileToolkit fileToolkit,
        ILogger<PromptExplorerService> logger)
    {
        _logger = logger;
        _fileToolkit = fileToolkit;
        _cacheList = new Dictionary<OnlinePromptSource, OnlinePromptList>();
    }

    /// <inheritdoc/>
    public async Task<OnlinePromptList> GetSourceDetailAsync(OnlinePromptSource source)
    {
        var hasCache = await TryLoadSourceAsync(source);
        return hasCache ? _cacheList[source] : default;
    }

    /// <inheritdoc/>
    public IEnumerable<OnlinePromptSource> GetSupportSources()
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

    /// <inheritdoc/>
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
            _logger.LogError(ex, $"An error occurred while trying to get {source}");
            return false;
        }
    }
}
