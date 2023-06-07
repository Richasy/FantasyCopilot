// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition for workflow step view model.
/// </summary>
public interface IWorkflowStepViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Current state.
    /// </summary>
    WorkflowStepState State { get; set; }

    /// <summary>
    /// Step data.
    /// </summary>
    WorkflowStep Step { get; }

    /// <summary>
    /// Step index.
    /// </summary>
    int Index { get; set; }

    /// <summary>
    /// Inejct step data.
    /// </summary>
    /// <param name="step">Step data.</param>
    void InjectStep(WorkflowStep step);
}
