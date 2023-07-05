// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Voice;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.UI.Dispatching;
using Windows.Media.Playback;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Text to speech module view model.
/// </summary>
public sealed partial class TextToSpeechModuleViewModel : ViewModelBase, ITextToSpeechModuleViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextToSpeechModuleViewModel"/> class.
    /// </summary>
    public TextToSpeechModuleViewModel(
        IVoiceService voiceService,
        ISettingsToolkit settingsToolkit,
        IResourceToolkit resourceToolkit,
        IAppViewModel appViewModel)
    {
        _voiceService = voiceService;
        _settingsToolkit = settingsToolkit;
        _resourceToolkit = resourceToolkit;
        _appViewModel = appViewModel;
        _allVoices = new List<VoiceMetadata>();
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _player = new MediaPlayer();
        _player.CurrentStateChanged += OnMediaPlayerStateChanged;
        IsPaused = true;
        SupportCultures = new SynchronizedObservableCollection<LocaleInfo>();
        DisplayVoices = new SynchronizedObservableCollection<VoiceMetadata>();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (!_voiceService.HasValidConfig || _allVoices.Count != 0)
        {
            return;
        }

        await ReloadMetadataAsync();
    }

    [RelayCommand]
    private async Task ReloadMetadataAsync()
    {
        IsMetadataLoading = true;
        TryClear(SupportCultures);
        try
        {
            _allVoices.Clear();
            var voices = await _voiceService.GetVoicesAsync();
            foreach (var vo in voices)
            {
                _allVoices.Add(vo);
            }

            var allCultures = _allVoices
                .Select(p => p.Locale)
                .Distinct()
                .Select(p => new LocaleInfo(new CultureInfo(p)))
                .OrderBy(p => p.Name)
                .ToList();
            foreach (var item in allCultures)
            {
                SupportCultures.Add(item);
            }

            var localLocale = new LocaleInfo(CultureInfo.CurrentCulture);
            var culture = allCultures.Contains(localLocale)
                ? localLocale
                : SupportCultures.FirstOrDefault();

            SelectedCulture = culture;
        }
        catch (System.Exception)
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.RequestVoiceMetadataFailed), InfoType.Error);
        }

        IsMetadataLoading = false;
    }

    [RelayCommand]
    private async Task ReadAsync()
    {
        if (IsConverting)
        {
            return;
        }

        _speechStream = default;
        if (string.IsNullOrEmpty(Text))
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.NeedInputText), InfoType.Error);
            return;
        }

        if (SelectedVoice == null)
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.NeedSelectVoice), InfoType.Error);
            return;
        }

        try
        {
            _speechStream?.Dispose();
            _speechStream = null;
            IsConverting = true;
            IsAudioEnabled = false;
            var stream = await _voiceService.GetSpeechAsync(Text, SelectedVoice.Id);
            if (stream == null)
            {
                _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.SpeechConvertFailed), InfoType.Error);
                return;
            }

            IsAudioEnabled = true;
            _speechStream = new MemoryStream();
            await stream.CopyToAsync(_speechStream);
            _player.SetStreamSource(stream.AsRandomAccessStream());
            _player.Play();
        }
        catch (TaskCanceledException)
        {
            // Do nothing.
        }
        catch (Exception)
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.SpeechConvertFailed), InfoType.Error);
        }

        IsConverting = false;
    }

    [RelayCommand]
    private async Task SaveAsync(string filePath)
    {
        if (_speechStream == null || string.IsNullOrEmpty(filePath))
        {
            return;
        }

        _speechStream.Seek(0, SeekOrigin.Begin);
        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await _speechStream.CopyToAsync(fileStream);
        _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.FileSaved), InfoType.Success);
    }

    [RelayCommand]
    private void PlayPause()
    {
        if (!_player.CanSeek)
        {
            return;
        }

        if (_player.CurrentState == MediaPlayerState.Playing)
        {
            _player.Pause();
        }
        else
        {
            _player.Play();
        }
    }

    [RelayCommand]
    private async Task ResetVoiceAsync()
    {
        TryClear(DisplayVoices);
        var voices = _allVoices
             .Where(p => p.Locale.Equals(SelectedCulture.Id, StringComparison.InvariantCultureIgnoreCase))
             .OrderBy(p => p.IsFemale)
             .ToList();
        foreach (var item in voices)
        {
            DisplayVoices.Add(item);
        }

        var voice = DisplayVoices.FirstOrDefault();

        await Task.Delay(100);
        SelectedVoice = voice;
    }

    private void OnMediaPlayerStateChanged(MediaPlayer sender, object args)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            IsPaused = sender.CurrentState != MediaPlayerState.Playing;
        });
    }

    partial void OnSelectedCultureChanged(LocaleInfo value)
    {
        if (value == null)
        {
            return;
        }

        ResetVoiceCommand.Execute(default);
    }
}
