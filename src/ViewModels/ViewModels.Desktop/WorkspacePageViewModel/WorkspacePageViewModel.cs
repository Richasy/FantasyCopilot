// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Workspace page view model.
/// </summary>
public sealed partial class WorkspacePageViewModel : ViewModelBase, IWorkspacePageViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkspacePageViewModel"/> class.
    /// </summary>
    public WorkspacePageViewModel(ISettingsToolkit settingsToolkit)
    {
        _settingsToolkit = settingsToolkit;
        SelectedType = _settingsToolkit.ReadLocalSetting(SettingNames.LastWorkspaceDataType, WorkspaceDataType.Workflows);
        CheckState();
    }

    private void CheckState()
    {
        IsWorkflowShown = SelectedType == WorkspaceDataType.Workflows;
        IsSemanticSkillShown = SelectedType == WorkspaceDataType.SemanticSkills;
        IsPluginShown = SelectedType == WorkspaceDataType.Plugins;
        IsImageSkillShown = SelectedType == WorkspaceDataType.ImageSkills;
    }

    partial void OnSelectedTypeChanged(WorkspaceDataType value)
    {
        _settingsToolkit.WriteLocalSetting(SettingNames.LastWorkspaceDataType, value);
        CheckState();
    }
}
