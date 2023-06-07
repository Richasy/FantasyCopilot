// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using FantasyCopilot.Models.App.Gpt;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Chat options view model interface.
/// </summary>
public interface ISessionOptionsViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Controls randomness: lowering temperature results in less random completions. At zero the model becomes deterministic.
    /// </summary>
    double Temperature { get; set; }

    /// <summary>
    /// The maximum number of tokens to generate in the output. One token is roughly 4 characters.
    /// </summary>
    int MaxResponseTokens { get; set; }

    /// <summary>
    /// Controls diversity via nucleus sampling: 0.5 means half of all likelihood-weighted options are considered.
    /// </summary>
    double TopP { get; set; }

    /// <summary>
    /// How much to penalize new tokens based on their existing frequency in the text so far.
    /// Decreases the model's likelihood to repeat the same line verbatim.
    /// </summary>
    double FrequencyPenalty { get; set; }

    /// <summary>
    /// How much to penalize new tokens based on whether they appear in the text so far.
    /// Increases the model's likelihood to talk about new topics.
    /// </summary>
    double PresencePenalty { get; set; }

    /// <summary>
    /// Initialize the view model.
    /// </summary>
    /// <param name="options">Existing configuration.</param>
    void Initialize(SessionOptions options = default);

    /// <summary>
    /// Get <see cref="SessionOptions"/> instance.
    /// </summary>
    /// <returns><see cref="SessionOptions"/>.</returns>
    SessionOptions GetOptions();
}
