// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Workspace page view model.
/// </summary>
public sealed partial class WorkspacePageViewModel
{
    private readonly ISettingsToolkit _settingsToolkit;

    [ObservableProperty]
    private bool _isWorkflowShown;

    [ObservableProperty]
    private bool _isSemanticSkillShown;

    [ObservableProperty]
    private bool _isPluginShown;

    [ObservableProperty]
    private bool _isImageSkillShown;

    [ObservableProperty]
    private WorkspaceDataType _selectedType;
}
