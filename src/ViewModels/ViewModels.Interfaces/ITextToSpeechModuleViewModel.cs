// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Voice;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition of text to speech module view model.
/// </summary>
public interface ITextToSpeechModuleViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Supported language.
    /// </summary>
    ObservableCollection<LocaleInfo> SupportCultures { get; }

    /// <summary>
    /// Voice list displayed.
    /// </summary>
    ObservableCollection<VoiceMetadata> DisplayVoices { get; }

    /// <summary>
    /// Selected language.
    /// </summary>
    LocaleInfo SelectedCulture { get; set; }

    /// <summary>
    /// Selected voice.
    /// </summary>
    VoiceMetadata SelectedVoice { get; set; }

    /// <summary>
    /// Text to convert.
    /// </summary>
    string Text { get; set; }

    /// <summary>
    /// Whether the metadata is loading.
    /// </summary>
    bool IsMetadataLoading { get; }

    /// <summary>
    /// Whether the text is being converted.
    /// </summary>
    bool IsConverting { get; }

    /// <summary>
    /// Is current player paused.
    /// </summary>
    bool IsPaused { get; }

    /// <summary>
    /// Audio is available.
    /// </summary>
    bool IsAudioEnabled { get; }

    /// <summary>
    /// Initialize module.
    /// </summary>
    IAsyncRelayCommand InitializeCommand { get; }

    /// <summary>
    /// Read text.
    /// </summary>
    IAsyncRelayCommand ReadCommand { get; }

    /// <summary>
    /// Save speech.
    /// </summary>
    IAsyncRelayCommand<string> SaveCommand { get; }

    /// <summary>
    /// Reload voice metadata.
    /// </summary>
    IAsyncRelayCommand ReloadMetadataCommand { get; }

    /// <summary>
    /// Play/pause the current speech.
    /// </summary>
    IRelayCommand PlayPauseCommand { get; }
}
