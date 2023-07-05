// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Workspace;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Workflow module view model.
/// </summary>
public interface IWorkflowsModuleViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Is the workflow list empty.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Is list loading.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Is editing config.
    /// </summary>
    bool IsEditing { get; set; }

    /// <summary>
    /// Is workflow running.
    /// </summary>
    bool IsRunning { get; set; }

    /// <summary>
    /// Workflow list.
    /// </summary>
    SynchronizedObservableCollection<WorkflowMetadata> Workflows { get; }

    /// <summary>
    /// Run config command.
    /// </summary>
    IRelayCommand<WorkflowMetadata> RunConfigCommand { get; }

    /// <summary>
    /// Edit config command.
    /// </summary>
    IRelayCommand<WorkflowMetadata> EditConfigCommand { get; }

    /// <summary>
    /// Delete config command.
    /// </summary>
    IAsyncRelayCommand<string> DeleteConfigCommand { get; }

    /// <summary>
    /// Initialize the list.
    /// </summary>
    IRelayCommand InitializeCommand { get; }

    /// <summary>
    /// Import config.
    /// </summary>
    IAsyncRelayCommand ImportCommand { get; }

    /// <summary>
    /// Export config.
    /// </summary>
    IAsyncRelayCommand ExportCommand { get; }
}
