// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.App.Gpt;

namespace FantasyCopilot.Models.App;

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
