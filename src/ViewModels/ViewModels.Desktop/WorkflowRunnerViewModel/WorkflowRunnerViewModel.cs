// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Workspace;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Libs.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Workflow runner view model.
/// </summary>
public sealed partial class WorkflowRunnerViewModel : ViewModelBase, IWorkflowRunnerViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkflowRunnerViewModel"/> class.
    /// </summary>
    public WorkflowRunnerViewModel(
        IWorkflowService workflowService,
        ICacheToolkit cacheToolkit,
        IResourceToolkit resourceToolkit,
        IAppViewModel appViewModel)
    {
        _workflowService = workflowService;
        _cacheToolkit = cacheToolkit;
        _resourceToolkit = resourceToolkit;
        _appViewModel = appViewModel;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        _workflowContext = Locator.Current.GetVariable<WorkflowContext>();
        Steps = new SynchronizedObservableCollection<IWorkflowStepViewModel>();
        AttachIsRunningToAsyncCommand(p => IsRunning = p, ExecuteCommand);
        _workflowContext.ResultUpdated += OnContextResultUpdated;
    }

    private static bool IsAdmin()
        => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    [RelayCommand]
    private async Task InjectMetadataAsync(WorkflowMetadata metadata)
    {
        _metadata = metadata;
        TryClear(Steps);
        Name = _metadata.Name;
        Description = _metadata.Description;
        NeedAdmin = false;
        ErrorText = string.Empty;
        Output = default;
        try
        {
            var detailData = await _cacheToolkit.GetWorkflowByIdAsync(metadata.Id);
            if (detailData.Steps.Any(p => p.Skill == SkillType.PluginCommand))
            {
                var plugins = await _cacheToolkit.GetPluginConfigsAsync();
                var commands = plugins.SelectMany(p => p.Commands);
                var stepRemoved = false;
                var isAdmin = IsAdmin();
                for (var i = detailData.Steps.Count - 1; i >= 0; i--)
                {
                    if (detailData.Steps[i].Skill == SkillType.PluginCommand)
                    {
                        var sourceCommand = commands.FirstOrDefault(p => p.Identity == detailData.Steps[i].PluginCommandId);
                        if (sourceCommand == null)
                        {
                            detailData.Steps.RemoveAt(i);
                            stepRemoved = true;
                        }
                        else if (sourceCommand.NeedAdmin && !isAdmin)
                        {
                            NeedAdmin = true;
                        }
                    }
                }

                if (stepRemoved)
                {
                    _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.InvalidStepRemoved), InfoType.Warning);
                }
            }

            var inputVM = Locator.Current.GetService<IWorkflowStepViewModel>();
            inputVM.InjectStep(detailData.Input);
            inputVM.State = WorkflowStepState.Input;
            Input = inputVM;
            if (detailData.Output != null)
            {
                var outputVM = Locator.Current.GetService<IWorkflowStepViewModel>();
                outputVM.InjectStep(detailData.Output);
                outputVM.State = WorkflowStepState.NotStarted;
                Output = outputVM;
            }

            foreach (var item in detailData.Steps)
            {
                var vm = Locator.Current.GetService<IWorkflowStepViewModel>();
                vm.InjectStep(item);
                vm.State = WorkflowStepState.NotStarted;
                Steps.Add(vm);
            }
        }
        catch (System.Exception)
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.WorkflowFileOpenFailed), InfoType.Error);
            Locator.Current.GetService<IWorkflowsModuleViewModel>().IsRunning = false;
        }
    }

    [RelayCommand]
    private async Task ExecuteAsync(string text)
    {
        if (_cancellationTokenSource != null && _cancellationTokenSource.Token.CanBeCanceled)
        {
            _cancellationTokenSource.Cancel();
        }

        if (Steps.Count == 0)
        {
            ErrorText = _resourceToolkit.GetLocalizedString(StringNames.NoStepError);
            return;
        }

        ErrorText = string.Empty;
        _cancellationTokenSource = new CancellationTokenSource();
        foreach (var item in Steps)
        {
            item.State = WorkflowStepState.NotStarted;
        }

        if (Output != null)
        {
            Output.State = WorkflowStepState.NotStarted;
        }

        var steps = Steps.Select(p => p.Step).ToList();
        Steps.First().State = WorkflowStepState.Running;
        var result = await _workflowService.ExecuteWorkflowAsync(text, steps, _cancellationTokenSource.Token);
        if (!result)
        {
            ErrorText = _workflowContext.StepResults[WorkflowConstants.ErrorKey];
        }
        else if (Output != null)
        {
            var finalResult = _workflowContext.StepResults.MaxBy(p => p.Key).Value;
            Output.Step.Detail = finalResult;
            Output.State = WorkflowStepState.Output;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        if (_cancellationTokenSource != null && _cancellationTokenSource.Token.CanBeCanceled)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = default;
        }
    }

    private void OnContextResultUpdated(object sender, EventArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            var maxIndex = _workflowContext.StepResults.Keys.Max();
            var hasError = _workflowContext.StepResults.ContainsKey(WorkflowConstants.ErrorKey);
            if (maxIndex == WorkflowConstants.ErrorKey)
            {
                Steps.First().State = WorkflowStepState.Error;
            }
            else
            {
                for (var i = 0; i < Steps.Count; i++)
                {
                    if (i <= maxIndex)
                    {
                        Steps[i].State = WorkflowStepState.Completed;
                    }
                    else if (i == maxIndex + 1)
                    {
                        Steps[i].State = hasError ? WorkflowStepState.Error : WorkflowStepState.Running;
                    }
                    else
                    {
                        Steps[i].State = WorkflowStepState.NotStarted;
                    }
                }
            }
        });
    }
}
