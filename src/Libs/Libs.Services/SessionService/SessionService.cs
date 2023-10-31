// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Gpt;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Session caching service.
/// </summary>
public sealed partial class SessionService : ServiceBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SessionService"/> class.
    /// </summary>
    private SessionService() => _messages = new List<Message>();

    /// <summary>
    /// Occurs when a new message is received.
    /// </summary>
    public event EventHandler<MessageReceivedEventArgs> MessageReceived;

    /// <summary>
    /// Fires when an error occurs.
    /// </summary>
    public event EventHandler<Exception> ExceptionThrown;

    /// <summary>
    /// Create new chat.
    /// </summary>
    /// <param name="systemPrompt">System prompt.</param>
    public static void CreateNewChat(string systemPrompt)
        => ChatService.Instance.CreateNewChat(systemPrompt);

    /// <summary>
    /// Update the message list cache.
    /// </summary>
    /// <param name="messages">Message list.</param>
    public void UpdateMessages(IEnumerable<Message> messages)
    {
        _messages.Clear();

        if (messages != null)
        {
            foreach (var message in messages)
            {
                _messages.Add(message);
            }
        }

        ChatService.Instance.SetHistory(_messages);
    }

    /// <summary>
    /// Update session configuration cache.
    /// </summary>
    /// <param name="options">New options.</param>
    public void UpdateSessionOptions(SessionOptions options)
        => _sessionOptions = options;

    /// <summary>
    /// Get full chat history, including implicit information.
    /// </summary>
    /// <returns><see cref="Message"/> list.</returns>
    public IEnumerable<Message> GetFullMessages()
        => _messages;

    /// <summary>
    /// Send message to LLM.
    /// </summary>
    /// <param name="message">User message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task SendMessageAsync(string message = null, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(message))
        {
            var userMsg = GetUserMessage(message);
            _messages.Add(userMsg);
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(new[] { userMsg }));
        }

        try
        {
            var msg = await GetResponseFromLLMAsync(cancellationToken);
            _messages.Add(msg);

            var args = new MessageReceivedEventArgs(new[] { msg });
            MessageReceived?.Invoke(this, args);
        }
        catch (Exception ex)
        {
            ExceptionThrown?.Invoke(this, ex);
            Logger.Error(ex, "An error occurred while trying to get the LLM response");
        }
    }

    /// <summary>
    /// Confirm that the MessageReceived event does not add an event callback.
    /// </summary>
    /// <returns>Returns <c>true</c> if no callback was added.</returns>
    public bool IsMessageReceivedEventNoHandler()
        => MessageReceived == null;

    /// <summary>
    /// Confirm that the ExceptionThrown event does not add an event callback.
    /// </summary>
    /// <returns>Returns <c>true</c> if no callback was added.</returns>
    public bool IsExceptionThrownEventNoHandler()
        => ExceptionThrown == null;

    /// <summary>
    /// Remove message item by index.
    /// </summary>
    public void RemoveMessage(int index)
    {
        if (_messages?.Count <= index)
        {
            return;
        }

        _messages.RemoveAt(index);
        ChatService.Instance.SetHistory(_messages);
    }

    /// <summary>
    /// User input must end with punctuation.
    /// Otherwise the returned content will be supplemented with punctuation at the beginning.
    /// </summary>
    /// <param name="message">Input text.</param>
    /// <returns>User message.</returns>
    private static Message GetUserMessage(string message)
    {
        var userMsg = new Message
        {
            IsUser = true,
            Content = message,
            Time = DateTimeOffset.Now,
        };

        return userMsg;
    }

    /// <summary>
    /// Get response from LLM.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Messages.</returns>
    /// <exception cref="Exception">Error exception.</exception>
    private async Task<Message> GetResponseFromLLMAsync(CancellationToken cancellationToken)
    {
        var sessionOptions = _sessionOptions;
        var message = _messages.Last();
        var data = await ChatService.Instance.GetMessageResponseAsync(message, sessionOptions, cancellationToken);
        return data.IsError
            ? throw new Exception(data.Content)
            : new Message
            {
                Content = data.Content,
                IsUser = false,
                Time = DateTimeOffset.Now,
                AdditionalMessage = data.AdditionalContent,
            };
    }
}
