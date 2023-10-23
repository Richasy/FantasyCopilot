// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using RichasyAssistant.Models.App.Workspace;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.ViewModels.Interfaces;

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
