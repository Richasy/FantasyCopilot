// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition of voice page view model.
/// </summary>
public interface IVoicePageViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Is text to speech checked.
    /// </summary>
    bool IsTextToSpeechSelected { get; set; }
}
