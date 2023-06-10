// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.UI.Dispatching;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Workflow module view model.
/// </summary>
public sealed partial class WorkflowsModuleViewModel : ViewModelBase, IWorkflowsModuleViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkflowsModuleViewModel"/> class.
    /// </summary>
    public WorkflowsModuleViewModel(ICacheToolkit cacheToolkit)
    {
        _cacheToolkit = cacheToolkit;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        Workflows = new ObservableCollection<WorkflowMetadata>();
        Workflows.CollectionChanged += OnWorkflowsCollectionChanged;
        _cacheToolkit.WorkflowListChanged += OnWorkflowListChanged;
        CheckIsEmpty();
    }

    [RelayCommand]
    private void Initialize()
    {
        if (IsLoading || _isInitialized)
        {
            return;
        }

        IsLoading = true;
        _dispatcherQueue.TryEnqueue(async () =>
        {
            TryClear(Workflows);
            var workflows = await _cacheToolkit.GetWorkflowListAsync();
            foreach (var workflow in workflows)
            {
                Workflows.Add(workflow);
            }

            _isInitialized = true;
            IsLoading = false;
        });
    }

    [RelayCommand]
    private void RunConfig(WorkflowMetadata config)
    {
        IsRunning = true;
        Locator.Current.GetService<IWorkflowRunnerViewModel>().InjectMetadataCommand.Execute(config);
    }

    [RelayCommand]
    private void EditConfig(WorkflowMetadata config)
    {
        IsEditing = true;
        Locator.Current.GetService<IWorkflowEditorViewModel>().InjectMetadataCommand.Execute(config);
    }

    [RelayCommand]
    private async Task DeleteConfigAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return;
        }

        var source = Workflows.FirstOrDefault(p => p.Id == id);
        if (source == null)
        {
            return;
        }

        Workflows.Remove(source);
        await _cacheToolkit.DeleteWorkflowAsync(id);
    }

    private void CheckIsEmpty()
        => IsEmpty = Workflows.Count == 0;

    private void OnWorkflowListChanged(object sender, EventArgs e)
    {
        _isInitialized = false;
        InitializeCommand.Execute(default);
    }

    private void OnWorkflowsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => CheckIsEmpty();

    partial void OnIsEditingChanged(bool value)
    {
        var appVM = Locator.Current.GetService<IAppViewModel>();
        appVM.IsBackButtonShown = value;
    }

    partial void OnIsRunningChanged(bool value)
    {
        var appVM = Locator.Current.GetService<IAppViewModel>();
        appVM.IsBackButtonShown = value;
    }
}
