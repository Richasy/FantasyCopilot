// Copyright (c) Fantasy Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Voice page view model.
/// </summary>
public sealed partial class VoicePageViewModel : ViewModelBase, IVoicePageViewModel
{
    [ObservableProperty]
    private bool _isTextToSpeechSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="VoicePageViewModel"/> class.
    /// </summary>
    public VoicePageViewModel()
        => IsTextToSpeechSelected = true;
}
