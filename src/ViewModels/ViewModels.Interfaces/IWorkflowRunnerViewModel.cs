﻿// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Workspace;

namespace RichasyAssistant.ViewModels.Interfaces;

/// <summary>
/// Interface definition for workflow runner view model.
/// </summary>
public interface IWorkflowRunnerViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Workflow name.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Workflow description.
    /// </summary>
    string Description { get; set; }

    /// <summary>
    /// Error text (If have).
    /// </summary>
    string ErrorText { get; set; }

    /// <summary>
    /// Is current workflow running.
    /// </summary>
    bool IsRunning { get; set; }

    /// <summary>
    /// The application needs to be restarted as Admin.
    /// </summary>
    bool NeedAdmin { get; set; }

    /// <summary>
    /// Input step.
    /// </summary>
    IWorkflowStepViewModel Input { get; set; }

    /// <summary>
    /// Output step.
    /// </summary>
    IWorkflowStepViewModel Output { get; set; }

    /// <summary>
    /// Workflow steps.
    /// </summary>
    SynchronizedObservableCollection<IWorkflowStepViewModel> Steps { get; }

    /// <summary>
    /// Inject workflow metadata and initialize.
    /// </summary>
    IAsyncRelayCommand<WorkflowMetadata> InjectMetadataCommand { get; }

    /// <summary>
    /// Execute the workflow.
    /// </summary>
    IAsyncRelayCommand<string> ExecuteCommand { get; }

    /// <summary>
    /// Cancel current running workflow.
    /// </summary>
    IRelayCommand CancelCommand { get; }
}
