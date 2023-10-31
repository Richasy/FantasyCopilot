// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using RichasyAssistant.Models.App.Voice;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Azure text to speech service.
/// </summary>
public sealed partial class AzureVoiceService : ServiceBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureVoiceService"/> class.
    /// </summary>
    private AzureVoiceService()
    {
    }

    /// <summary>
    /// Occurs when speech is recognizing.
    /// </summary>
    public event EventHandler<string> SpeechRecognizing;

    /// <summary>
    /// Occurs when speech is recognized.
    /// </summary>
    public event EventHandler<string> SpeechRecognized;

    /// <summary>
    /// Occurs when speech is stopped.
    /// </summary>
    public event EventHandler RecognizeStopped;

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static AzureVoiceService Instance { get; } = new AzureVoiceService();

    /// <summary>
    /// Has valid config.
    /// </summary>
    public bool HasValidConfig => _hasValidConfig;

    /// <summary>
    /// Generate speech from text.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="voiceId">Voice id.</param>
    /// <returns>Audio stream.</returns>
    /// <exception cref="System.InvalidCastException">Can not convert to stream.</exception>
    /// <exception cref="TaskCanceledException">User cancel the task.</exception>
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
                    Logger.Error($"Speech cancelled: ${cancellation.ErrorCode}, ${cancellation.ErrorDetails}");
                    throw new InvalidCastException("Something wrong");
                }
                else if (cancellation.Reason == CancellationReason.CancelledByUser)
                {
                    throw new TaskCanceledException();
                }

                break;
            default:
                Logger.Error($"Speech not completed: ${result.Reason}");
                break;
        }

        return default;
    }

    /// <summary>
    /// Get voices from online.
    /// </summary>
    /// <returns>Voice list.</returns>
    public async Task<List<VoiceMetadata>> GetVoicesAsync()
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

    /// <summary>
    /// Do a short speech recognition.
    /// </summary>
    /// <returns>Recognized content.</returns>
    public async Task<string> RecognizeOnceAsync(string locale)
    {
        InitializeSpeechRecognizer(locale);
        var result = await _speechRecognizer.RecognizeOnceAsync();
        if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = CancellationDetails.FromResult(result);
            if (cancellation.Reason == CancellationReason.Error)
            {
                Logger.Error($"Speech recognition interrupted: {cancellation.ErrorCode}, {cancellation.ErrorDetails}");
                throw new System.Exception("Recognize failed.");
            }
        }

        return result.Text ?? string.Empty;
    }

    /// <summary>
    /// Start continuous speech recognition.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task StartRecognizingAsync(string locale)
    {
        InitializeSpeechRecognizer(locale);
        _speechRecognizer.Recognizing += OnSpeechRecognizerRecognizing;
        _speechRecognizer.Recognized += OnSpeechRecognizerRecognized;
        _speechRecognizer.SessionStopped += OnSpeechSessionStopped;
        _speechRecognizer.Canceled += OnSpeechSessionCanceled;
        Logger.Info("Speech recognition activated");
        await _speechRecognizer.StartContinuousRecognitionAsync();
        Logger.Info("Speech recognition is off");
    }

    /// <summary>
    /// Start continuous speech recognition.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public Task StopRecognizeAsync()
        => _speechRecognizer.StopContinuousRecognitionAsync();

    /// <summary>
    /// Reload config.
    /// </summary>
    public void ReloadConfig()
    {
        _speechConfig = default;
        CheckConfig();
    }
}
