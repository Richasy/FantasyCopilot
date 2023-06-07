// Copyright (c) Fantasy Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Workflow step view model.
/// </summary>
public sealed partial class WorkflowStepViewModel : ViewModelBase, IWorkflowStepViewModel
{
    [ObservableProperty]
    private WorkflowStepState _state;

    [ObservableProperty]
    private WorkflowStep _step;

    [ObservableProperty]
    private int _index;

    /// <inheritdoc/>
    public void InjectStep(WorkflowStep step)
    {
        Step = step;
        Index = step.Index;
    }

    partial void OnIndexChanged(int value)
        => Step.Index = value;
}
