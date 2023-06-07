// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Gpt;

namespace FantasyCopilot.Services.Interfaces;

/// <summary>
/// Session caching service interface.
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Occurs when a new message is received.
    /// </summary>
    event EventHandler<MessageReceivedEventArgs> MessageReceived;

    /// <summary>
    /// Fires when an error occurs.
    /// </summary>
    event EventHandler<Exception> ExceptionThrown;

    /// <summary>
    /// Update session configuration cache.
    /// </summary>
    /// <param name="options">New options.</param>
    void UpdateSessionOptions(SessionOptions options);

    /// <summary>
    /// Update the message list cache.
    /// </summary>
    /// <param name="messages">Message list.</param>
    void UpdateMessages(IEnumerable<Message> messages);

    /// <summary>
    /// Send message to LLM.
    /// </summary>
    /// <param name="message">User message.</param>
    /// <param name="isContext">Whether to look up the context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task SendMessageAsync(string message = default, bool isContext = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create new chat.
    /// </summary>
    /// <param name="systemPrompt">System prompt.</param>
    void CreateNewChat(string systemPrompt);

    /// <summary>
    /// Get full chat history, including implicit information.
    /// </summary>
    /// <returns><see cref="Message"/> list.</returns>
    IEnumerable<Message> GetFullMessages();

    /// <summary>
    /// Remove message item by index.
    /// </summary>
    void RemoveMessage(int index);

    /// <summary>
    /// Confirm that the MessageReceived event does not add an event callback.
    /// </summary>
    /// <returns>Returns <c>true</c> if no callback was added.</returns>
    bool IsMessageReceivedEventNoHandler();

    /// <summary>
    /// Confirm that the ExceptionThrown event does not add an event callback.
    /// </summary>
    /// <returns>Returns <c>true</c> if no callback was added.</returns>
    bool IsExceptionThrownEventNoHandler();
}
