// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Voice;
using RichasyAssistant.Libs.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;
using Windows.Media.Playback;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Text to speech module view model.
/// </summary>
public sealed partial class TextToSpeechModuleViewModel
{
    private readonly IVoiceService _voiceService;
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IAppViewModel _appViewModel;
    private readonly List<VoiceMetadata> _allVoices;
    private readonly MediaPlayer _player;
    private readonly DispatcherQueue _dispatcherQueue;

    private Stream _speechStream;
    private string _preloadText;

    [ObservableProperty]
    private LocaleInfo _selectedCulture;

    [ObservableProperty]
    private VoiceMetadata _selectedVoice;

    [ObservableProperty]
    private string _text;

    [ObservableProperty]
    private bool _isMetadataLoading;

    [ObservableProperty]
    private bool _isConverting;

    [ObservableProperty]
    private bool _isPaused;

    [ObservableProperty]
    private bool _isAudioEnabled;

    /// <inheritdoc/>
    public ObservableCollection<LocaleInfo> SupportCultures { get; }

    /// <inheritdoc/>
    public ObservableCollection<VoiceMetadata> DisplayVoices { get; }
}
