// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.Toolkits.Interfaces;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Workflow module view model.
/// </summary>
public sealed partial class WorkflowsModuleViewModel
{
    private readonly ICacheToolkit _cacheToolkit;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private bool _isRunning;

    private bool _isInitialized;

    /// <inheritdoc />
    public ObservableCollection<WorkflowMetadata> Workflows { get; }
}
