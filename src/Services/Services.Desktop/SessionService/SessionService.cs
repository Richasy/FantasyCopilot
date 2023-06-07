// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using Microsoft.Extensions.Logging;

namespace FantasyCopilot.Services;

/// <summary>
/// Session caching service.
/// </summary>
public sealed partial class SessionService : ISessionService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SessionService"/> class.
    /// </summary>
    public SessionService(
        ISettingsToolkit settingsToolkit,
        ICacheToolkit cacheToolkit,
        IMemoryService memoryService,
        IChatService chatService,
        ILogger<SessionService> logger)
    {
        _settingsToolkit = settingsToolkit;
        _cacheToolkit = cacheToolkit;
        _memoryService = memoryService;
        _chatService = chatService;
        _logger = logger;
        _messages = new List<Message>();
    }

    /// <inheritdoc/>
    public event EventHandler<MessageReceivedEventArgs> MessageReceived;

    /// <inheritdoc/>
    public event EventHandler<Exception> ExceptionThrown;

    /// <inheritdoc/>
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

        _chatService.SetHistory(_messages);
    }

    /// <inheritdoc/>
    public void UpdateSessionOptions(SessionOptions options)
        => _sessionOptions = options;

    /// <inheritdoc/>
    public IEnumerable<Message> GetFullMessages()
        => _messages;

    /// <inheritdoc/>
    public void CreateNewChat(string systemPrompt)
        => _chatService.CreateNewChat(systemPrompt);

    /// <inheritdoc/>
    public async Task SendMessageAsync(string message = null, bool isContext = false, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(message))
        {
            var userMsg = GetUserMessage(message);
            _messages.Add(userMsg);
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(new[] { userMsg }));
        }

        try
        {
            var msg = await GetResponseFromLLMAsync(isContext, cancellationToken);
            _messages.Add(msg);

            var args = new MessageReceivedEventArgs(new[] { msg });
            MessageReceived?.Invoke(this, args);
        }
        catch (Exception ex)
        {
            ExceptionThrown?.Invoke(this, ex);
            _logger.LogError(ex, "An error occurred while trying to get the LLM response");
        }
    }

    /// <inheritdoc/>
    public bool IsMessageReceivedEventNoHandler()
        => MessageReceived == null;

    /// <inheritdoc/>
    public bool IsExceptionThrownEventNoHandler()
        => ExceptionThrown == null;

    /// <inheritdoc/>
    public void RemoveMessage(int index)
    {
        if (_messages?.Count <= index)
        {
            return;
        }

        _messages.RemoveAt(index);
        _chatService.SetHistory(_messages);
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
    /// <param name="isContext">Is context search.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Messages.</returns>
    /// <exception cref="Exception">Error exception.</exception>
    private async Task<Message> GetResponseFromLLMAsync(bool isContext, CancellationToken cancellationToken)
    {
        var sessionOptions = _sessionOptions;
        var message = _messages.Last();
        var data = isContext
            ? await _memoryService.SearchMemoryAsync(message.Content, sessionOptions, cancellationToken)
            : await _chatService.GetMessageResponseAsync(message, sessionOptions, cancellationToken);
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
