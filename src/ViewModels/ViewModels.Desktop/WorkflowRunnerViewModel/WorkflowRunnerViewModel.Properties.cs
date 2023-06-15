// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.UI.Dispatching;

namespace FantasyCopilot.ViewModels;

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
    public ObservableCollection<IWorkflowStepViewModel> Steps { get; }
}
