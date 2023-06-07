// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.App.Gpt;

namespace FantasyCopilot.Services.Interfaces;

/// <summary>
/// A service that handles chat, based on <see cref="IKernelService"/>.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Create a new chat.
    /// </summary>
    /// <param name="systemPrompt">System prompt.</param>
    void CreateNewChat(string systemPrompt);

    /// <summary>
    /// Update chat history.
    /// </summary>
    /// <param name="history">Chat history.</param>
    void SetHistory(IEnumerable<Message> history);

    /// <summary>
    /// Get chat message response.
    /// </summary>
    /// <param name="message">New message.</param>
    /// <param name="options">Session options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="MessageResponse"/>.</returns>
    Task<MessageResponse> GetMessageResponseAsync(Message message, SessionOptions options, CancellationToken cancellationToken);
}
