// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.UI.Dispatching;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Speech recognition module view model.
/// </summary>
public sealed partial class SpeechRecognizeModuleViewModel
{
    private readonly IVoiceService _voiceService;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IAppViewModel _appViewModel;
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly List<string> _cacheTextList;

    [ObservableProperty]
    private bool _isContinuous;

    [ObservableProperty]
    private bool _isRecording;

    [ObservableProperty]
    private string _text;

    [ObservableProperty]
    private LocaleInfo _selectedCulture;

    [ObservableProperty]
    private bool _isInitializing;

    /// <inheritdoc/>
    public ObservableCollection<LocaleInfo> SupportCultures { get; }
}
