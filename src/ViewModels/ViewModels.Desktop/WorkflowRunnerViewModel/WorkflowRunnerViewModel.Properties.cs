// Copyright (c) Richasy Assistant. All rights reserved.

using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Workspace;
using RichasyAssistant.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Workflow runner view model.
/// </summary>
public sealed partial class WorkflowRunnerViewModel
{
    private readonly IWorkflowService _workflowService;
    private readonly ICacheToolkit _cacheToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IAppViewModel _appViewModel;
    private readonly WorkflowContext _workflowContext;
    private readonly DispatcherQueue _dispatcherQueue;

    private WorkflowMetadata _metadata;
    private CancellationTokenSource _cancellationTokenSource;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private bool _needAdmin;

    [ObservableProperty]
    private IWorkflowStepViewModel _input;

    [ObservableProperty]
    private IWorkflowStepViewModel _output;

    /// <inheritdoc/>
    public SynchronizedObservableCollection<IWorkflowStepViewModel> Steps { get; }
}
