// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FantasyCopilot.Models.App.Voice;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Logging;

namespace FantasyCopilot.Services;

/// <summary>
/// Azure text to speech service.
/// </summary>
public sealed partial class AzureVoiceService : IVoiceService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureVoiceService"/> class.
    /// </summary>
    public AzureVoiceService(
        ISettingsToolkit settingsToolkit,
        IFileToolkit fileToolkit,
        ILogger<AzureVoiceService> logger)
    {
        _logger = logger;
        _settingsToolkit = settingsToolkit;
        _fileToolkit = fileToolkit;
    }

    /// <inheritdoc/>
    public event EventHandler<string> SpeechRecognizing;

    /// <inheritdoc/>
    public event EventHandler<string> SpeechRecognized;

    /// <inheritdoc/>
    public event EventHandler RecognizeStopped;

    /// <inheritdoc/>
    public bool HasValidConfig => _hasValidConfig;

    /// <inheritdoc/>
    public async Task<Stream> GetSpeechAsync(string text, string voiceId)
    {
        CheckConfig();
        _speechConfig.SpeechSynthesisVoiceName = voiceId;
        using var speech = new SpeechSynthesizer(_speechConfig, default);
        using var result = await speech.SpeakTextAsync(text);
        switch (result.Reason)
        {
            case ResultReason.SynthesizingAudioCompleted:
                var ms = new MemoryStream(result.AudioData);
                return ms;
            case ResultReason.Canceled:
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                if (cancellation.Reason == CancellationReason.Error)
                {
                    _logger.LogError($"Speech cancelled: ${cancellation.ErrorCode}, ${cancellation.ErrorDetails}");
                    throw new System.InvalidCastException("Something wrong");
                }
                else if (cancellation.Reason == CancellationReason.CancelledByUser)
                {
                    throw new TaskCanceledException();
                }

                break;
            default:
                _logger.LogError($"Speech not completed: ${result.Reason}");
                break;
        }

        return default;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<VoiceMetadata>> GetVoicesAsync()
    {
        var data = await _fileToolkit.GetDataFromFileAsync<List<VoiceMetadata>>(AppConstants.AzureVoicesFileName, default);
        if (data == null || data.Count == 0)
        {
            var voices = await GetVoicesFromOnlineAsync();
            if (voices == null)
            {
                return default;
            }

            data = voices.Select(p => new VoiceMetadata
            {
                Id = p.ShortName,
                Name = p.LocalName,
                IsFemale = p.Gender == SynthesisVoiceGender.Female,
                IsNeural = p.VoiceType.ToString().Contains("Neural"),
                Locale = p.Locale,
            })
                .OrderBy(p => p.Id)
                .ToList();
            await _fileToolkit.WriteContentAsync(System.Text.Json.JsonSerializer.Serialize(data), AppConstants.AzureVoicesFileName);
        }

        return data;
    }

    /// <inheritdoc/>
    public async Task<string> RecognizeOnceAsync(string locale)
    {
        InitializeSpeechRecognizer(locale);
        var result = await _speechRecognizer.RecognizeOnceAsync();
        if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = CancellationDetails.FromResult(result);
            if (cancellation.Reason == CancellationReason.Error)
            {
                _logger.LogError($"Speech recognition interrupted: {cancellation.ErrorCode}, {cancellation.ErrorDetails}");
                throw new System.Exception("Recognize failed.");
            }
        }

        return result.Text ?? string.Empty;
    }

    /// <inheritdoc/>
    public async Task StartRecognizingAsync(string locale)
    {
        InitializeSpeechRecognizer(locale);
        _speechRecognizer.Recognizing += OnSpeechRecognizerRecognizing;
        _speechRecognizer.Recognized += OnSpeechRecognizerRecognized;
        _speechRecognizer.SessionStopped += OnSpeechSessionStopped;
        _speechRecognizer.Canceled += OnSpeechSessionCanceled;
        _logger.LogInformation("Speech recognition activated");
        await _speechRecognizer.StartContinuousRecognitionAsync();
        _logger.LogInformation("Speech recognition is off");
    }

    /// <inheritdoc/>
    public Task StopRecognizeAsync()
        => _speechRecognizer.StopContinuousRecognitionAsync();

    /// <inheritdoc/>
    public void ReloadConfig()
    {
        _speechConfig = default;
        CheckConfig();
    }
}
