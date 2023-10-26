// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Speech recognition module view model.
/// </summary>
public sealed partial class SpeechRecognizeModuleViewModel : ViewModelBase, ISpeechRecognizeModuleViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechRecognizeModuleViewModel"/> class.
    /// </summary>
    public SpeechRecognizeModuleViewModel(
        IVoiceService voiceService,
        IResourceToolkit resourceToolkit,
        ISettingsToolkit settingsToolkit,
        IAppViewModel appViewModel)
    {
        _voiceService = voiceService;
        _resourceToolkit = resourceToolkit;
        _settingsToolkit = settingsToolkit;
        _appViewModel = appViewModel;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        SupportCultures = new ObservableCollection<LocaleInfo>();
        _cacheTextList = new List<string>();
        IsContinuous = _settingsToolkit.ReadLocalSetting(SettingNames.ContinuousSpeechRecognize, false);
        _voiceService.SpeechRecognizing += OnSpeechRecognizing;
        _voiceService.SpeechRecognized += OnSpeechRecognized;
        _voiceService.RecognizeStopped += OnRecognizeStopped;

        AttachIsRunningToAsyncCommand(p => IsInitializing = p, InitializeCommand);
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        IsRecording = true;
        Text = string.Empty;
        _cacheTextList.Clear();
        if (IsContinuous)
        {
            await _voiceService.StartRecognizingAsync(SelectedCulture?.Id);
        }
        else
        {
            try
            {
                Text = await _voiceService.RecognizeOnceAsync(SelectedCulture?.Id);
                IsRecording = false;
            }
            catch (System.Exception)
            {
                _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.VoiceRecognizeFailed), InfoType.Error);
            }
        }
    }

    [RelayCommand]
    private async Task StopAsync()
    {
        IsRecording = false;
        await _voiceService.StopRecognizeAsync();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (SupportCultures.Count > 0)
        {
            return;
        }

        var voices = await _voiceService.GetVoicesAsync();
        var allCultures = voices
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
        SelectedCulture = allCultures.Contains(localLocale)
            ? localLocale
            : SupportCultures.FirstOrDefault();
    }

    private void OnSpeechRecognizing(object sender, string e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            FormatText(e);
        });
    }

    private void OnSpeechRecognized(object sender, string e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            _cacheTextList.Add(e);
            FormatText(string.Empty);
        });
    }

    private void OnRecognizeStopped(object sender, EventArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            IsRecording = false;
        });
    }

    private void FormatText(string text)
    {
        var t = string.Join("\n", _cacheTextList);
        Text = string.IsNullOrEmpty(t) ? text : t + "\n" + text;
    }

    partial void OnIsContinuousChanged(bool value)
        => _settingsToolkit.WriteLocalSetting(SettingNames.ContinuousSpeechRecognize, value);
}
