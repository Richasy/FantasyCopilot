// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition for workflow editor view model.
/// </summary>
public interface IWorkflowEditorViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Triggered after the step command finishes loading.
    /// </summary>
    event EventHandler StepCommandsLoaded;

    /// <summary>
    /// Workflow name.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Workflow description.
    /// </summary>
    string Goal { get; set; }

    /// <summary>
    /// Is steps empty.
    /// </summary>
    bool IsStepEmpty { get; }

    /// <summary>
    /// Whether to show the auto-plan button.
    /// </summary>
    bool IsInspireButtonShown { get; }

    /// <summary>
    /// Is it being automatically scheduled.
    /// </summary>
    bool IsInspiring { get; }

    /// <summary>
    /// Whether the step command is loading.
    /// </summary>
    bool IsStepCommandsLoading { get; }

    /// <summary>
    /// Input step.
    /// </summary>
    IWorkflowStepViewModel Input { get; set; }

    /// <summary>
    /// Output step.
    /// </summary>
    IWorkflowStepViewModel Output { get; set; }

    /// <summary>
    /// The command group used for the workflow step.
    /// </summary>
    IReadOnlyList<WorkCommandBase> StepCommands { get; }

    /// <summary>
    /// The command group used for the workflow input.
    /// </summary>
    IReadOnlyList<WorkCommandBase> InputCommands { get; }

    /// <summary>
    /// The command group used for the workflow output.
    /// </summary>
    IReadOnlyList<WorkCommandBase> OutputCommands { get; }

    /// <summary>
    /// Workflow steps.
    /// </summary>
    SynchronizedObservableCollection<IWorkflowStepViewModel> Steps { get; }

    /// <summary>
    /// Inject workflow metadata and initialize.
    /// </summary>
    IAsyncRelayCommand<WorkflowMetadata> InjectMetadataCommand { get; }

    /// <summary>
    /// Save the workflow.
    /// </summary>
    IAsyncRelayCommand SaveCommand { get; }

    /// <summary>
    /// Create new step.
    /// </summary>
    IRelayCommand<WorkCommandItem> CreateStepCommand { get; }

    /// <summary>
    /// Create new input.
    /// </summary>
    IRelayCommand<SkillType> CreateInputCommand { get; }

    /// <summary>
    /// Create new output.
    /// </summary>
    IRelayCommand<SkillType> CreateOutputCommand { get; }

    /// <summary>
    /// Remove the specific step.
    /// </summary>
    IRelayCommand<int> RemoveStepCommand { get; }

    /// <summary>
    /// Move step up.
    /// </summary>
    IRelayCommand<IWorkflowStepViewModel> MoveUpwardCommand { get; }

    /// <summary>
    /// Move step down.
    /// </summary>
    IRelayCommand<IWorkflowStepViewModel> MoveDownwardCommand { get; }

    /// <summary>
    /// Try to generate steps based on existing workflow goal.
    /// </summary>
    IAsyncRelayCommand InspireCommand { get; }
}
