// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;

namespace RichasyAssistant.ViewModels.Interfaces;

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
