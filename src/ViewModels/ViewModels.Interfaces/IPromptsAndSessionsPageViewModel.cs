﻿// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.ViewModels.Interfaces;

/// <summary>
/// Interface definition of prompts page view model.
/// </summary>
public interface IPromptsAndSessionsPageViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Is favorite prompt checked.
    /// </summary>
    bool IsFavoritePromptsShown { get; }

    /// <summary>
    /// Is prompt library checked.
    /// </summary>
    bool IsPromptLibraryShown { get; }

    /// <summary>
    /// Is saved sessions checked.
    /// </summary>
    bool IsSavedSessionsShown { get; }

    /// <summary>
    /// Current selected type.
    /// </summary>
    ChatDataType SelectedType { get; set; }
}
