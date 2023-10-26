// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Prompts page view model.
/// </summary>
public sealed partial class PromptsAndSessionsPageViewModel
{
    private readonly ISettingsToolkit _settingsToolkit;

    [ObservableProperty]
    private ChatDataType _selectedType;

    [ObservableProperty]
    private bool _isFavoritePromptsShown;

    [ObservableProperty]
    private bool _isPromptLibraryShown;

    [ObservableProperty]
    private bool _isSavedSessionsShown;
}
