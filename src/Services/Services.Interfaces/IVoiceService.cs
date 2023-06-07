// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.App.Voice;

namespace FantasyCopilot.Services.Interfaces;

/// <summary>
/// Interface definition of text to speech service.
/// </summary>
public interface IVoiceService : IConfigServiceBase
{
    /// <summary>
    /// Occurs when speech recognizing.
    /// </summary>
    event EventHandler<string> SpeechRecognizing;

    /// <summary>
    /// Occurs when speech recognized.
    /// </summary>
    event EventHandler<string> SpeechRecognized;

    /// <summary>
    /// Occurs when speech recognition stopped.
    /// </summary>
    event EventHandler RecognizeStopped;

    /// <summary>
    /// Get all voices.
    /// </summary>
    /// <returns>Voice list.</returns>
    Task<IEnumerable<VoiceMetadata>> GetVoicesAsync();

    /// <summary>
    /// Read input text.
    /// </summary>
    /// <param name="text">Text to be read aloud.</param>
    /// <param name="voiceId">Voice id.</param>
    /// <returns>Speech stream.</returns>
    Task<Stream> GetSpeechAsync(string text, string voiceId);

    /// <summary>
    /// Do a short speech recognition.
    /// </summary>
    /// <returns>Recognized content.</returns>
    Task<string> RecognizeOnceAsync(string locale);

    /// <summary>
    /// Start continuous speech recognition.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task StartRecognizingAsync(string locale);

    /// <summary>
    /// End continuous speech recognition and return the recognized text.
    /// </summary>
    /// <returns>Recognized content.</returns>
    Task StopRecognizeAsync();
}
