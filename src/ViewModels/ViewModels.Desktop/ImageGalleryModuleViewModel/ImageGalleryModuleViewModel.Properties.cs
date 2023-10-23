// Copyright (c) Richasy Assistant. All rights reserved.

using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Image gallery module view model.
/// </summary>
public sealed partial class ImageGalleryModuleViewModel
{
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly ICivitaiService _civitaiService;
    private readonly IAppViewModel _appViewModel;
    private readonly ILogger<ImageGalleryModuleViewModel> _logger;

    private int _currentPage;
    private CancellationTokenSource _cancellationTokenSource;

    [ObservableProperty]
    private CivitaiSortType _currentSortType;

    [ObservableProperty]
    private CivitaiPeriodType _currentPeriodType;

    [ObservableProperty]
    private bool _isInitializing;

    [ObservableProperty]
    private bool _isIncrementalLoading;

    [ObservableProperty]
    private bool _hasNext;

    [ObservableProperty]
    private bool _isEmpty;

    /// <inheritdoc/>
    public SynchronizedObservableCollection<ICivitaiImageViewModel> Images { get; }
}
