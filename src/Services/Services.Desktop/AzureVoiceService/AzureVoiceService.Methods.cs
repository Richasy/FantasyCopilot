// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Logging;

namespace FantasyCopilot.Services;

/// <summary>
/// Azure voice service.
/// </summary>
public sealed partial class AzureVoiceService
{
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IFileToolkit _fileToolkit;
    private readonly ILogger<AzureVoiceService> _logger;
    private SpeechConfig _speechConfig;
    private bool _hasValidConfig;
    private SpeechRecognizer _speechRecognizer;

    /// <inheritdoc/>
    public bool NeedDependencies { get; set; }

    private async Task CheckConfigAsync()
    {
        var voiceKey = await _settingsToolkit.RetrieveSecureStringAsync(SettingNames.AzureVoiceKey);
        var hasRegion = _settingsToolkit.IsSettingKeyExist(SettingNames.AzureVoiceRegion);

        _hasValidConfig = !string.IsNullOrEmpty(voiceKey) && hasRegion;

        if (_hasValidConfig && _speechConfig == null)
        {
            try
            {
                var region = _settingsToolkit.ReadLocalSetting(SettingNames.AzureVoiceRegion, string.Empty);
                _speechConfig = SpeechConfig.FromSubscription(voiceKey, region);
                NeedDependencies = false;
            }
            catch (DllNotFoundException)
            {
                // Users need to be guided to download C++ dependencies.
                NeedDependencies = true;
                _hasValidConfig = false;
            }
        }
    }

    private async Task<List<VoiceInfo>> GetVoicesFromOnlineAsync()
    {
        using var speech = new SpeechSynthesizer(_speechConfig);
        using var result = await speech.GetVoicesAsync();
        var data = result?.Voices?.ToList();
        return data;
    }

    private async Task InitializeSpeechRecognizerAsync(string locale)
    {
        await CheckConfigAsync();
        if (!string.IsNullOrEmpty(locale))
        {
            _speechConfig.SpeechRecognitionLanguage = locale;
        }

        var autoDetectSourceLanguageConfig = AutoDetectSourceLanguageConfig.FromLanguages(
                new string[] { "en-US", "zh-CN" });
        var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        _speechRecognizer = new SpeechRecognizer(_speechConfig, autoDetectSourceLanguageConfig, audioConfig);
    }

    private void OnSpeechRecognizerRecognizing(object sender, SpeechRecognitionEventArgs e)
    {
        var text = e.Result.Text;
        if (!string.IsNullOrEmpty(text))
        {
            SpeechRecognizing?.Invoke(this, text);
        }
    }

    private void OnSpeechSessionCanceled(object sender, SpeechRecognitionCanceledEventArgs e)
        => RecognizeStopped?.Invoke(this, EventArgs.Empty);

    private void OnSpeechSessionStopped(object sender, SessionEventArgs e)
        => RecognizeStopped?.Invoke(this, EventArgs.Empty);

    private void OnSpeechRecognizerRecognized(object sender, SpeechRecognitionEventArgs e)
    {
        var text = e.Result.Text;
        if (!string.IsNullOrEmpty(text))
        {
            SpeechRecognized?.Invoke(this, text);
        }
    }
}
