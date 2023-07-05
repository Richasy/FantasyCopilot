// Copyright (c) Fantasy Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;

namespace FantasyCopilot.ViewModels;

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
