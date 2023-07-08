// Copyright (c) Fantasy Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Knowledge;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Knowledge page view model.
/// </summary>
public sealed partial class KnowledgePageViewModel
{
    private readonly ICacheToolkit _cacheToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IMemoryService _memoryService;
    private readonly IAppViewModel _appViewModel;
    private readonly IKnowledgeBaseSessionViewModel _knowledgeBaseSessionViewModel;
    private readonly ILogger<KnowledgePageViewModel> _logger;
    private readonly DispatcherTimer _progressTimer;
    private readonly DispatcherQueue _dispatcherQueue;

    private bool _isInitialized;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isBaseConnecting;

    [ObservableProperty]
    private bool _isBaseCreating;

    [ObservableProperty]
    private int _totalFileCount;

    [ObservableProperty]
    private int _importedFileCount;

    [ObservableProperty]
    private KnowledgeBase _currentBase;

    /// <inheritdoc/>
    public SynchronizedObservableCollection<IKnowledgeBaseItemViewModel> Bases { get; set; }
}
