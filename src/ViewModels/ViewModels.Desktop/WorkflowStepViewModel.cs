// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using RichasyAssistant.Models.App.Workspace;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

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
