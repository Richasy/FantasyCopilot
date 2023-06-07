// Copyright (c) Fantasy Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;

namespace FantasyCopilot.ViewModels;

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
