// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Prompts page view model.
/// </summary>
public sealed partial class PromptsAndSessionsPageViewModel : ViewModelBase, IPromptsAndSessionsPageViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptsAndSessionsPageViewModel"/> class.
    /// </summary>
    public PromptsAndSessionsPageViewModel(ISettingsToolkit settingsToolkit)
    {
        _settingsToolkit = settingsToolkit;
        SelectedType = _settingsToolkit.ReadLocalSetting(SettingNames.LastChatDataType, ChatDataType.FavoritePrompts);
        CheckState();
    }

    private void CheckState()
    {
        IsFavoritePromptsShown = SelectedType == ChatDataType.FavoritePrompts;
        IsPromptLibraryShown = SelectedType == ChatDataType.PromptLibrary;
        IsSavedSessionsShown = SelectedType == ChatDataType.SavedSessions;
    }

    partial void OnSelectedTypeChanged(ChatDataType value)
    {
        _settingsToolkit.WriteLocalSetting(SettingNames.LastChatDataType, value);
        CheckState();
    }
}
