// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App.Gpt;

/// <summary>
/// Session options.
/// </summary>
public class SessionOptions
{
    /// <summary>
    /// Controls randomness: lowering temperature results in less random completions. At zero the model becomes deterministic.
    /// </summary>
    public double Temperature { get; set; }

    /// <summary>
    /// The maximum number of tokens to generate in the output. One token is roughly 4 characters.
    /// </summary>
    public int MaxResponseTokens { get; set; }

    /// <summary>
    /// Controls diversity via nucleus sampling: 0.5 means half of all likelihood-weighted options are considered.
    /// </summary>
    public double TopP { get; set; }

    /// <summary>
    /// How much to penalize new tokens based on their existing frequency in the text so far.
    /// Decreases the model's likelihood to repeat the same line verbatim.
    /// </summary>
    public double FrequencyPenalty { get; set; }

    /// <summary>
    /// How much to penalize new tokens based on whether they appear in the text so far.
    /// Increases the model's likelihood to talk about new topics.
    /// </summary>
    public double PresencePenalty { get; set; }

    /// <summary>
    /// Use streaming output (typewriter mode).
    /// </summary>
    public bool UseStreamOutput { get; set; }

    /// <summary>
    /// Automatically remove earlier messages when the token limit is reached.
    /// </summary>
    public bool AutoRemoveEarlierMessage { get; set; }

    /// <summary>
    /// Create a copy based on its own properties.
    /// </summary>
    /// <returns>New <see cref="SessionOptions"/> object.</returns>
    public SessionOptions Clone()
        => new SessionOptions
        {
            Temperature = Temperature,
            MaxResponseTokens = MaxResponseTokens,
            TopP = TopP,
            FrequencyPenalty = FrequencyPenalty,
            PresencePenalty = PresencePenalty,
            UseStreamOutput = UseStreamOutput,
            AutoRemoveEarlierMessage = AutoRemoveEarlierMessage,
        };
}
