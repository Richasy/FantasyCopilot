// Copyright (c) Fantasy Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;

namespace FantasyCopilot.ViewModels;

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
