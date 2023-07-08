// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.Models.Constants;
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
    public WorkflowsModuleViewModel(
        ICacheToolkit cacheToolkit,
        IResourceToolkit resourceToolkit,
        IAppViewModel appViewModel)
    {
        _cacheToolkit = cacheToolkit;
        _resourceToolkit = resourceToolkit;
        _appViewModel = appViewModel;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        Workflows = new SynchronizedObservableCollection<WorkflowMetadata>();
        Workflows.CollectionChanged += OnWorkflowsCollectionChanged;
        _cacheToolkit.WorkflowListChanged += OnWorkflowListChanged;
        CheckIsEmpty();
    }

    [RelayCommand]
    private void Initialize()
    {
        _dispatcherQueue.TryEnqueue(async () =>
        {
            if (IsLoading || _isInitialized)
            {
                return;
            }

            IsLoading = true;
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

    [RelayCommand]
    private async Task ImportAsync()
    {
        var result = await _cacheToolkit.ImportWorkflowsAsync();
        if (result == null)
        {
            return;
        }
        else if (result.Value)
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.DataImported), InfoType.Success);
        }
        else
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.ImportDataFailed), InfoType.Error);
        }
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        var result = await _cacheToolkit.ExportWorkflowsAsync();
        if (result == null)
        {
            return;
        }
        else if (result.Value)
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.DataExported), InfoType.Success);
        }
        else
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.ExportDataFailed), InfoType.Error);
        }
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
