// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Gpt;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// View model of the saved sessions page.
/// </summary>
public sealed partial class SavedSessionsModuleViewModel
{
    private readonly ICacheToolkit _cacheToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IAppViewModel _appViewModel;
    private readonly DispatcherQueue _dispatcherQueue;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    private bool _isInitialized;

    /// <inheritdoc/>
    public SynchronizedObservableCollection<SessionMetadata> Sessions { get; }
}
