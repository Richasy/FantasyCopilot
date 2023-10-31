// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Workspace;
using RichasyAssistant.Libs.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Workflow editor view model.
/// </summary>
public sealed partial class WorkflowEditorViewModel
{
    private readonly ICacheToolkit _cacheToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IWorkflowService _workflowService;
    private readonly IAppViewModel _appViewModel;

    private WorkflowMetadata _metadata;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _goal;

    [ObservableProperty]
    private bool _isStepEmpty;

    [ObservableProperty]
    private bool _isInspireButtonShown;

    [ObservableProperty]
    private bool _isInspiring;

    [ObservableProperty]
    private bool _isStepCommandsLoading;

    [ObservableProperty]
    private IWorkflowStepViewModel _input;

    [ObservableProperty]
    private IWorkflowStepViewModel _output;

    /// <inheritdoc/>
    public event EventHandler StepCommandsLoaded;

    /// <inheritdoc/>
    public IReadOnlyList<WorkCommandBase> StepCommands { get; set; }

    /// <inheritdoc/>
    public IReadOnlyList<WorkCommandBase> InputCommands { get; set; }

    /// <inheritdoc/>
    public IReadOnlyList<WorkCommandBase> OutputCommands { get; set; }

    /// <inheritdoc/>
    public SynchronizedObservableCollection<IWorkflowStepViewModel> Steps { get; }
}
