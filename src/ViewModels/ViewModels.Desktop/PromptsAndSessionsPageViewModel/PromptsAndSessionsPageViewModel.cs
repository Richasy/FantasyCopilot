// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

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
