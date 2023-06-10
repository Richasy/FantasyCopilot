// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace FantasyCopilot.Libs.NativeSkills;

/// <summary>
/// Chat skill.
/// </summary>
public sealed class ChatSkill
{
    private readonly ILogger<ChatSkill> _logger;
    private readonly Timer _respondTimer;
    private IChatCompletion _chatCompletion;
    private ChatHistory _chatHistory;
    private ChatRequestSettings _chatRequestSettings;
    private string _systemPrompt;
    private int _waitingMilliseconds = 0;
    private bool _autoRemoveEarlierMessage = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSkill"/> class.
    /// </summary>
    public ChatSkill()
    {
        var kernel = Locator.Current.GetVariable<IKernel>();
        _logger = Locator.Current.GetLogger<ChatSkill>();
        Locator.Current.VariableChanged += OnVariableChanged;
        _chatCompletion = kernel.GetService<IChatCompletion>();
        _respondTimer = new Timer(TimeSpan.FromMilliseconds(10));
        _respondTimer.Elapsed += (_, _) => _waitingMilliseconds += 10;
    }

    /// <summary>
    /// Occurs when new content is received while waiting for an LLM response.
    /// </summary>
    public event EventHandler<string> CharacterReceived;

    /// <summary>
    /// Initialize current chat.
    /// </summary>
    /// <param name="context">Current context.</param>
    /// <returns><see cref="Task"/>.</returns>
    [SKFunction(WorkflowConstants.Chat.InitializeDescription)]
    [SKFunctionName(WorkflowConstants.Chat.InitializeName)]
    public Task InitializeAsync(SKContext context)
    {
        context.Variables.TryGetValue(AppConstants.SessionOptionsKey, out string optionsStr);
        var options = JsonSerializer.Deserialize<SessionOptions>(optionsStr);
        _autoRemoveEarlierMessage = options.AutoRemoveEarlierMessage;
        _chatRequestSettings = new ChatRequestSettings()
        {
            TopP = options.TopP,
            FrequencyPenalty = options.FrequencyPenalty,
            PresencePenalty = options.PresencePenalty,
            MaxTokens = options.MaxResponseTokens,
            Temperature = options.Temperature,
        };

        return Task.CompletedTask;
    }

    /// <summary>
    /// Send message to LLM.
    /// </summary>
    /// <param name="context">Current context.</param>
    /// <returns>Message response.</returns>
    [SKFunction(WorkflowConstants.Chat.SendDescription)]
    [SKFunctionName(WorkflowConstants.Chat.SendName)]
    public async Task<string> SendAsync(SKContext context)
    {
        var reply = string.Empty;
        try
        {
            _chatHistory.AddMessage(AuthorRole.System, context.Result);
            reply = await _chatCompletion.GenerateMessageAsync(_chatHistory, _chatRequestSettings, context.CancellationToken);

            // If the response is empty, remove the last sent message.
            if (string.IsNullOrEmpty(reply))
            {
                _chatHistory.RemoveAt(_chatHistory.Count - 1);
            }
            else
            {
                _chatHistory.AddMessage(AuthorRole.Assistant, reply);
            }
        }
        catch (AIException e)
        {
            _logger.LogError(e, "Chat skill error.");
            _chatHistory.Remove(_chatHistory.LastOrDefault(p => p.Role == AuthorRole.User));
            var retried = false;
            if (_autoRemoveEarlierMessage
                && e.ErrorCode == AIException.ErrorCodes.InvalidRequest
                && e.Detail.TryGetTokenLimit(out var maxTokens, out var msgTokens))
            {
                _logger.LogInformation("Older messages have been removed, retrying");
                var canRetry = StaticHelpers.TryRemoveEarlierMessage(_chatHistory, maxTokens, msgTokens);
                if (canRetry)
                {
                    retried = true;
                    reply = await SendAsync(context);
                }
            }

            if (!retried)
            {
                reply = $"{AppConstants.ExceptionTag}{e.Detail}{AppConstants.ExceptionTag}";
            }
        }

        return reply;
    }

    /// <summary>
    /// Send message to LLM.
    /// </summary>
    /// <param name="context">Current context.</param>
    /// <returns>Message response.</returns>
    [SKFunction(WorkflowConstants.Chat.GenerateStreamDescription)]
    [SKFunctionName(WorkflowConstants.Chat.GenerateStreamName)]
    public async Task<string> GenerateStreamAsync(SKContext context)
    {
        var reply = string.Empty;
        try
        {
            _chatHistory.AddMessage(AuthorRole.User, context.Result);
            var response = _chatCompletion.GenerateMessageStreamAsync(_chatHistory, _chatRequestSettings, context.CancellationToken);

            await foreach (var msg in response)
            {
                if (!_respondTimer.Enabled)
                {
                    _respondTimer.Start();
                }

                if (string.IsNullOrEmpty(msg))
                {
                    continue;
                }

                reply += msg;

                if (_waitingMilliseconds > 50)
                {
                    _waitingMilliseconds = 0;
                    CharacterReceived?.Invoke(this, reply);
                }
            }

            reply = reply.Trim();

            // If the response is empty, remove the last sent message.
            if (string.IsNullOrEmpty(reply))
            {
                _chatHistory.RemoveAt(_chatHistory.Count - 1);
            }
            else
            {
                _chatHistory.AddMessage(AuthorRole.Assistant, reply);
            }
        }
        catch (AIException e)
        {
            _logger.LogError(e, "Chat skill error.");
            _chatHistory.Remove(_chatHistory.LastOrDefault(p => p.Role == AuthorRole.User));
            var retried = false;
            if (_autoRemoveEarlierMessage
                && e.ErrorCode == AIException.ErrorCodes.InvalidRequest
                && e.Detail.TryGetTokenLimit(out var maxTokens, out var msgTokens))
            {
                _logger.LogInformation("Older messages have been removed, retrying");
                var msgs = _chatHistory;
                var canRetry = StaticHelpers.TryRemoveEarlierMessage(_chatHistory, maxTokens, msgTokens);
                if (canRetry)
                {
                    retried = true;
                    _chatHistory.Remove(_chatHistory.LastOrDefault(p => p.Role == AuthorRole.User));
                    reply = await GenerateStreamAsync(context);
                }
            }

            if (!retried)
            {
                reply = $"{AppConstants.ExceptionTag}{e.Detail}{AppConstants.ExceptionTag}";
            }
        }

        _respondTimer.Stop();
        return reply;
    }

    /// <summary>
    /// Create new chat with prompt.
    /// </summary>
    /// <param name="prompt">System prompt.</param>
    public void Create(string prompt)
    {
        _systemPrompt = prompt;
        _chatHistory = _chatCompletion?.CreateNewChat(prompt);
    }

    /// <summary>
    /// Set up chat history.
    /// </summary>
    /// <param name="messages">Chat history.</param>
    public void SetHistory(IEnumerable<Message> messages)
    {
        _chatHistory?.Messages.Clear();
        if (messages == null || _chatHistory == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(_systemPrompt))
        {
            _chatHistory.AddMessage(AuthorRole.System, _systemPrompt);
        }

        foreach (var message in messages)
        {
            if (message.IsUser)
            {
                _chatHistory.AddMessage(AuthorRole.User, message.Content);
            }
            else
            {
                _chatHistory.AddMessage(AuthorRole.Assistant, message.Content);
            }
        }
    }

    private void OnVariableChanged(object sender, Type e)
    {
        if (e == typeof(IKernel))
        {
            var kernel = Locator.Current.GetVariable<IKernel>();
            var kernelService = Locator.Current.GetService<IKernelService>();
            if (!kernelService.HasChatModel)
            {
                return;
            }

            var chatCompletion = kernel.GetService<IChatCompletion>();
            var chatHistory = chatCompletion.CreateNewChat(string.Empty);
            if (_chatHistory != null)
            {
                var messages = _chatHistory;
                foreach (var item in messages)
                {
                    chatHistory.AddMessage(item.Role, item.Content ?? string.Empty);
                }
            }

            _chatCompletion = chatCompletion;
            _chatHistory = chatHistory;
        }
    }
}
