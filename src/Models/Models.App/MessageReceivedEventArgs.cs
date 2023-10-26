// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.Gpt;

namespace RichasyAssistant.Models.App;

/// <summary>
/// Message received event argument.
/// </summary>
public sealed class MessageReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageReceivedEventArgs"/> class.
    /// </summary>
    public MessageReceivedEventArgs(IEnumerable<Message> messages)
        => Messages = messages;

    /// <summary>
    /// Received messages.
    /// </summary>
    public IEnumerable<Message> Messages { get; }
}
