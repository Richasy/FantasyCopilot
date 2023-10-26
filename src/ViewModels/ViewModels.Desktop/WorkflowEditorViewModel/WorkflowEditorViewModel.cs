// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Plugins;
using RichasyAssistant.Models.App.Workspace;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Workflow editor view model.
/// </summary>
public sealed partial class WorkflowEditorViewModel : ViewModelBase, IWorkflowEditorViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkflowEditorViewModel"/> class.
    /// </summary>
    public WorkflowEditorViewModel(
        ICacheToolkit cacheToolkit,
        IResourceToolkit resourceToolkit,
        IWorkflowService workflowService,
        IAppViewModel appViewModel)
    {
        _cacheToolkit = cacheToolkit;
        _resourceToolkit = resourceToolkit;
        _workflowService = workflowService;
        _appViewModel = appViewModel;

        InitializeInputCommands();
        InitializeOutputCommands();
        Steps = new SynchronizedObservableCollection<IWorkflowStepViewModel>();
        Steps.CollectionChanged += OnStepsCollectionChanged;
        AttachIsRunningToAsyncCommand(p => IsInspiring = p, InspireCommand);
        IsStepEmpty = true;
    }

    private static WorkCommandItem GetCommandItem(PluginCommand command, string pluginId)
        => new WorkCommandItem(command.Name, SkillType.PluginCommand, pluginId, command.Identity);

    private WorkCommandItem GetCommandItem(SkillType skill, StringNames name)
        => new WorkCommandItem(
            _resourceToolkit.GetLocalizedString(name),
            skill);

    private WorkCommandGroup GetCommandGroup(StringNames name, List<WorkCommandBase> items)
        => new WorkCommandGroup(
            _resourceToolkit.GetLocalizedString(name),
            items);

    [RelayCommand]
    private async Task InjectMetadataAsync(WorkflowMetadata metadata)
    {
        _metadata = metadata;
        TryClear(Steps);

        await InitializeStepCommandsAsync();

        if (_metadata == default)
        {
            Name = string.Empty;
            Goal = string.Empty;
            Input = default;
            Output = default;
        }
        else
        {
            Name = _metadata.Name;
            Goal = _metadata.Description;
            Output = default;
            try
            {
                var detailData = await _cacheToolkit.GetWorkflowByIdAsync(metadata.Id);
                var inputVM = Locator.Current.GetService<IWorkflowStepViewModel>();
                inputVM.InjectStep(detailData.Input);
                Input = inputVM;
                if (detailData.Output != null)
                {
                    var outputVM = Locator.Current.GetService<IWorkflowStepViewModel>();
                    outputVM.InjectStep(detailData.Output);
                    Output = outputVM;
                }

                foreach (var item in detailData.Steps)
                {
                    var vm = Locator.Current.GetService<IWorkflowStepViewModel>();
                    vm.InjectStep(item);
                    Steps.Add(vm);
                }

                ResetStepIndex();
            }
            catch (Exception)
            {
                _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.WorkflowFileOpenFailed), InfoType.Error);
            }
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(Name)
            || string.IsNullOrEmpty(Goal)
            || Input?.Step == default)
        {
            _appViewModel.ShowTip(
                _resourceToolkit.GetLocalizedString(StringNames.NeedFillRequiredFields),
                InfoType.Warning);
            return;
        }

        if (Steps.Count == 0)
        {
            _appViewModel.ShowTip(
                _resourceToolkit.GetLocalizedString(StringNames.NeedAddStep),
                InfoType.Warning);
            return;
        }

        _metadata ??= new WorkflowMetadata();

        if (string.IsNullOrEmpty(_metadata.Id))
        {
            _metadata.Id = Guid.NewGuid().ToString("N");
        }

        _metadata.Name = Name;
        _metadata.Description = Goal;
        var detailData = new WorkflowConfig
        {
            Id = _metadata.Id,
            Input = Input.Step,
            Steps = Steps.Select(p => p.Step).ToList(),
            Output = Output?.Step,
        };

        await _cacheToolkit.SaveWorkflowAsync(detailData);
        await _cacheToolkit.AddOrUpdateWorkflowMetadataAsync(_metadata);
        await InjectMetadataAsync(default);
        Locator.Current.GetService<IWorkflowsModuleViewModel>().IsEditing = false;
    }

    [RelayCommand]
    private void CreateStep(WorkCommandItem command)
    {
        var step = new WorkflowStep
        {
            Skill = command.Skill,
            Index = Steps.Count,
        };

        if (command.Skill == SkillType.PluginCommand)
        {
            step.PluginCommandId = command.CommandId;
        }

        var vm = Locator.Current.GetService<IWorkflowStepViewModel>();
        vm.InjectStep(step);
        vm.State = WorkflowStepState.Configuring;
        Steps.Add(vm);
    }

    [RelayCommand]
    private void CreateInput(SkillType type)
    {
        var step = new WorkflowStep
        {
            Skill = type,
            Index = -1,
        };

        var vm = Locator.Current.GetService<IWorkflowStepViewModel>();
        vm.InjectStep(step);
        vm.State = WorkflowStepState.Configuring;
        Input = vm;
    }

    [RelayCommand]
    private void CreateOutput(SkillType type)
    {
        var step = new WorkflowStep
        {
            Skill = type,
            Index = -1,
        };

        var vm = Locator.Current.GetService<IWorkflowStepViewModel>();
        vm.InjectStep(step);
        vm.State = WorkflowStepState.Configuring;
        Output = vm;
    }

    [RelayCommand]
    private void RemoveStep(int index)
    {
        if (index >= Steps.Count || index < 0)
        {
            return;
        }

        Steps.RemoveAt(index);
        for (var i = 0; i < Steps.Count; i++)
        {
            Steps[i].Index = i;
        }
    }

    [RelayCommand]
    private void MoveUpward(IWorkflowStepViewModel step)
    {
        var targetIndex = step.Index - 1;
        MoveStep(step.Index, targetIndex);
    }

    [RelayCommand]
    private void MoveDownward(IWorkflowStepViewModel step)
    {
        var targetIndex = step.Index + 1;
        MoveStep(step.Index, targetIndex);
    }

    [RelayCommand]
    private async Task InspireAsync()
    {
        if (string.IsNullOrEmpty(Goal))
        {
            return;
        }

        var data = await _workflowService.GetWorkflowStepsFromGoalAsync(Goal);
        if (data?.Any() ?? false)
        {
            foreach (var step in data)
            {
                var vm = Locator.Current.GetService<IWorkflowStepViewModel>();
                vm.InjectStep(step);

                if (step.Skill.GetHashCode() < 100)
                {
                    if (Input != default)
                    {
                        continue;
                    }

                    Input = vm;
                }
                else if (step.Skill.GetHashCode() < 1000)
                {
                    if (Output != default)
                    {
                        continue;
                    }

                    Output = vm;
                }
                else
                {
                    Steps.Add(vm);
                }
            }
        }
        else
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.NoAutoPlan), InfoType.Warning);
        }

        ResetStepIndex();
    }

    private void MoveStep(int oldIndex, int newIndex)
    {
        if (newIndex < 0 || newIndex >= Steps.Count)
        {
            return;
        }

        Steps.Move(oldIndex, newIndex);
        ResetStepIndex();
    }

    private void ResetStepIndex()
    {
        for (var i = 0; i < Steps.Count; i++)
        {
            Steps[i].Index = i;
        }
    }

    private void InitializeInputCommands()
    {
        var list = new List<WorkCommandBase>
        {
            GetCommandItem(SkillType.InputText, StringNames.TextInput),
            GetCommandItem(SkillType.InputVoice, StringNames.VoiceInput),
            GetCommandItem(SkillType.InputFile, StringNames.FileInput),
            GetCommandItem(SkillType.InputClick, StringNames.ClickInput),
        };

        InputCommands = list.AsReadOnly();
    }

    private void InitializeOutputCommands()
    {
        var list = new List<WorkCommandBase>
        {
            GetCommandItem(SkillType.OutputText, StringNames.OutputText),
            GetCommandItem(SkillType.OutputVoice, StringNames.OutputVoice),
            GetCommandItem(SkillType.OutputImage, StringNames.OutputImage),
        };

        OutputCommands = list.AsReadOnly();
    }

    private async Task InitializeStepCommandsAsync()
    {
        var list = new List<WorkCommandBase>
        {
            GetCommandItem(SkillType.Semantic, StringNames.SemanticSkill),
        };

        var textGroup = GetCommandGroup(StringNames.Text, new List<WorkCommandBase>
            {
                GetCommandItem(SkillType.Translate, StringNames.Translate),
                GetCommandItem(SkillType.TextOverwrite, StringNames.TextOverwrite),
                GetCommandItem(SkillType.VariableRedirect, StringNames.VariableRedirect),
                GetCommandItem(SkillType.VariableCreate, StringNames.CreateVariable),
            });
        var fileGroup = GetCommandGroup(StringNames.File, new List<WorkCommandBase>());
        var imgGroup = GetCommandGroup(StringNames.Image, new List<WorkCommandBase>
            {
                GetCommandItem(SkillType.TextToImage, StringNames.TextToImage),
            });
        var videoGroup = GetCommandGroup(StringNames.Video, new List<WorkCommandBase>());
        var voiceGroup = GetCommandGroup(StringNames.Voice, new List<WorkCommandBase>
            {
                GetCommandItem(SkillType.TextToSpeech, StringNames.TextToSpeech),
            });
        var webGroup = GetCommandGroup(StringNames.Web, new List<WorkCommandBase>());
        var nativeGroup = GetCommandGroup(StringNames.Native, new List<WorkCommandBase>
            {
                GetCommandItem(SkillType.TextNotification, StringNames.TextNotification),
            });
        var a11yGroup = GetCommandGroup(StringNames.Accessibility, new List<WorkCommandBase>());
        var toolGroup = GetCommandGroup(StringNames.Tool, new List<WorkCommandBase>());
        var knowledgeGroup = GetCommandGroup(StringNames.KnowledgeBase, new List<WorkCommandBase>
            {
                GetCommandItem(SkillType.GetKnowledge, StringNames.KnowledgeBaseQA),
                GetCommandItem(SkillType.ImportFileToKnowledge, StringNames.ImportFile),
                GetCommandItem(SkillType.ImportFolderToKnowledge, StringNames.ImportFolder),
            });
        var otherGroup = GetCommandGroup(StringNames.Other, new List<WorkCommandBase>());

        var plugins = await _cacheToolkit.GetPluginConfigsAsync();
        if (plugins.Any())
        {
            foreach (var item in plugins)
            {
                var pluginId = item.Id;
                if (item.Commands?.Any() ?? false)
                {
                    foreach (var command in item.Commands)
                    {
                        var commandItem = GetCommandItem(command, pluginId);

                        var targetGroup = command.Category switch
                        {
                            WorkflowConstants.TextGroupName => textGroup,
                            WorkflowConstants.FileGroupName => fileGroup,
                            WorkflowConstants.ImageGroupName => imgGroup,
                            WorkflowConstants.VideoGroupName => videoGroup,
                            WorkflowConstants.VoiceGroupName => voiceGroup,
                            WorkflowConstants.WebGroupName => webGroup,
                            WorkflowConstants.NativeGroupName => nativeGroup,
                            WorkflowConstants.A11yGroupName => a11yGroup,
                            WorkflowConstants.ToolGroupName => toolGroup,
                            WorkflowConstants.KnowledgeGroupName => knowledgeGroup,
                            _ => otherGroup,
                        };

                        targetGroup.Commands.Add(commandItem);
                    }
                }
            }
        }

        TryAddGroup(textGroup);
        TryAddGroup(knowledgeGroup);
        TryAddGroup(fileGroup);
        TryAddGroup(imgGroup);
        TryAddGroup(videoGroup);
        TryAddGroup(voiceGroup);
        TryAddGroup(webGroup);
        TryAddGroup(nativeGroup);
        TryAddGroup(a11yGroup);
        TryAddGroup(toolGroup);
        TryAddGroup(otherGroup);

        StepCommands = list.AsReadOnly();
        StepCommandsLoaded?.Invoke(this, EventArgs.Empty);

        void TryAddGroup(WorkCommandGroup group)
        {
            if (group.Commands.Count == 0)
            {
                return;
            }

            list.Add(group);
        }
    }

    private void CheckInspireButtonState()
        => IsInspireButtonShown = !string.IsNullOrEmpty(Goal) && IsStepEmpty;

    private void OnStepsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => IsStepEmpty = Steps.Count == 0;

    partial void OnIsStepEmptyChanged(bool value)
        => CheckInspireButtonState();

    partial void OnGoalChanged(string value)
        => CheckInspireButtonState();
}
