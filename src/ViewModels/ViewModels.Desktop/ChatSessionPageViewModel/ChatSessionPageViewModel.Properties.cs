// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Chat session page view model.
/// </summary>
public sealed partial class ChatSessionPageViewModel
{
    [ObservableProperty]
    private ISessionViewModel _currentSession;

    [ObservableProperty]
    private bool _isInSettings;
}
