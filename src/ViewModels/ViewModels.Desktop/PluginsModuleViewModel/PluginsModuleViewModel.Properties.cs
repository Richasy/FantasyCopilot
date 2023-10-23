// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using RichasyAssistant.Models.App;
using RichasyAssistant.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Plugins module view model.
/// </summary>
public sealed partial class PluginsModuleViewModel
{
    private readonly IWorkflowService _workflowService;
    private readonly ICacheToolkit _cacheToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IFileToolkit _fileToolkit;
    private readonly IAppViewModel _appViewModel;
    private readonly ILogger<PluginsModuleViewModel> _logger;

    private bool _isInitialized;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isImporting;

    /// <inheritdoc/>
    public SynchronizedObservableCollection<IPluginItemViewModel> Plugins { get; set; }
}
