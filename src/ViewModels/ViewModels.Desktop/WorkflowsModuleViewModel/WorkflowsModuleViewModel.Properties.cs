// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Workspace;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Workflow module view model.
/// </summary>
public sealed partial class WorkflowsModuleViewModel
{
    private readonly ICacheToolkit _cacheToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IAppViewModel _appViewModel;
    private readonly DispatcherQueue _dispatcherQueue;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private bool _isRunning;

    private bool _isInitialized;

    /// <inheritdoc />
    public SynchronizedObservableCollection<WorkflowMetadata> Workflows { get; }
}
