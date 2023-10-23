// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichasyAssistant.Models.App;

namespace RichasyAssistant.ViewModels.Interfaces;

/// <summary>
/// Interface definition for the speech recognition module view model.
/// </summary>
public interface ISpeechRecognizeModuleViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Whether it is continuous recognition.
    /// </summary>
    bool IsContinuous { get; set; }

    /// <summary>
    /// Is recording.
    /// </summary>
    bool IsRecording { get; }

    /// <summary>
    /// Is initializing.
    /// </summary>
    bool IsInitializing { get; }

    /// <summary>
    /// Recognized text.
    /// </summary>
    string Text { get; set; }

    /// <summary>
    /// Selected culture.
    /// </summary>
    LocaleInfo SelectedCulture { get; set; }

    /// <summary>
    /// Support cultures.
    /// </summary>
    ObservableCollection<LocaleInfo> SupportCultures { get; }

    /// <summary>
    /// Start speech recognition.
    /// </summary>
    IAsyncRelayCommand StartCommand { get; }

    /// <summary>
    /// Stop speech recognition.
    /// </summary>
    IAsyncRelayCommand StopCommand { get; }

    /// <summary>
    /// Initialize module.
    /// </summary>
    IAsyncRelayCommand InitializeCommand { get; }
}
