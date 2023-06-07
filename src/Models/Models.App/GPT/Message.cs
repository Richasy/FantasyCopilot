// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.App.Gpt;

/// <summary>
/// Chat message structure.
/// </summary>
public sealed class Message
{
    /// <summary>
    /// Is it a message sent by the user.
    /// </summary>
    public bool IsUser { get; set; }

    /// <summary>
    /// Message content for display.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Time the message was sent.
    /// </summary>
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// Additional information, such as data sources.
    /// </summary>
    public string AdditionalMessage { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Message message && Time.Equals(message.Time);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Time);
}
