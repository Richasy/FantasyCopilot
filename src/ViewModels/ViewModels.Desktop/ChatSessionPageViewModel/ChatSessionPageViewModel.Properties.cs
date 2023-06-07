// Copyright (c) Fantasy Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

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
