// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

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
