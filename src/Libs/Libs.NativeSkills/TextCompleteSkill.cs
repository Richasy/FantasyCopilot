// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace FantasyCopilot.Libs.NativeSkills;

/// <summary>
/// Text complete skill.
/// </summary>
public class TextCompleteSkill
{
    private readonly ILogger<TextCompleteSkill> _logger;
    private readonly List<ChatMessageBase> _completeHistory;
    private readonly Timer _respondTimer;
    private ITextCompletion _textCompletion;
    private CompleteRequestSettings _completeRequestSettings;
    private string _systemPrompt;
    private int _waitingMilliseconds = 0;
    private bool _autoRemoveEarlierMessage = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextCompleteSkill"/> class.
    /// </summary>
    public TextCompleteSkill()
    {
        var kernel = Locator.Current.GetVariable<IKernel>();
        _logger = Locator.Current.GetLogger<TextCompleteSkill>();
        _completeHistory = new List<ChatMessageBase>();
        Locator.Current.VariableChanged += OnVariableChanged;
        _textCompletion = kernel.GetService<ITextCompletion>();
        _respondTimer = new Timer(TimeSpan.FromMilliseconds(10));
        _respondTimer.Elapsed += (_, _) => _waitingMilliseconds += 10;
    }

    /// <summary>
    /// Occurs when new content is received while waiting for an LLM response.
    /// </summary>
    public event EventHandler<string> CharacterReceived;

    /// <summary>
    /// Initialize current session.
    /// </summary>
    /// <param name="context">Current context.</param>
    /// <returns><see cref="Task"/>.</returns>
    [Description(WorkflowConstants.TextCompletion.InitializeDescription)]
    [SKName(WorkflowConstants.TextCompletion.InitializeName)]
    [SKFunction]
    public Task InitializeAsync(SKContext context)
    {
        context.Variables.TryGetValue(AppConstants.SessionOptionsKey, out string optionsStr);
        var options = JsonSerializer.Deserialize<SessionOptions>(optionsStr);
        _completeRequestSettings = new CompleteRequestSettings()
        {
            TopP = options.TopP,
            FrequencyPenalty = options.FrequencyPenalty,
            PresencePenalty = options.PresencePenalty,
            MaxTokens = options.MaxResponseTokens,
            Temperature = options.Temperature,
        };

        _autoRemoveEarlierMessage = options.AutoRemoveEarlierMessage;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Complete the given text.
    /// </summary>
    /// <param name="context">Current context.</param>
    /// <returns>Message response.</returns>
    [Description(WorkflowConstants.TextCompletion.CompleteDescription)]
    [SKName(WorkflowConstants.TextCompletion.CompleteName)]
    [SKFunction]
    public async Task<string> CompleteAsync(SKContext context)
    {
        var reply = string.Empty;
        try
        {
            _completeHistory.Add(new ChatMessage(AuthorRole.User, context.Result));
            var previousText = GenerateContextString();
            reply = await _textCompletion.CompleteAsync(previousText, _completeRequestSettings, context.CancellationToken);
            reply = reply.Replace(previousText, string.Empty).Trim();

            // If the response is empty, remove the last sent message.
            if (string.IsNullOrEmpty(reply))
            {
                _completeHistory.RemoveAt(_completeHistory.Count - 1);
            }
            else
            {
                _completeHistory.Add(new ChatMessage(AuthorRole.Assistant, reply));
            }
        }
        catch (AIException e)
        {
            _logger.LogError(e, "Text Completion skill error.");
            _completeHistory.Remove(_completeHistory.LastOrDefault(p => p.Role == AuthorRole.User));
            var retried = false;
            if (_autoRemoveEarlierMessage
                && e.ErrorCode == AIException.ErrorCodes.InvalidRequest
                && e.Detail.TryGetTokenLimit(out var maxTokens, out var msgTokens))
            {
                _logger.LogInformation("Older messages have been removed, retrying");
                var canRetry = StaticHelpers.TryRemoveEarlierMessage(_completeHistory, maxTokens, msgTokens);
                if (canRetry)
                {
                    retried = true;
                    _completeHistory.Remove(_completeHistory.LastOrDefault(p => p.Role == AuthorRole.User));
                    reply = await CompleteAsync(context);
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
    /// Complete the given text.
    /// </summary>
    /// <param name="context">Current context.</param>
    /// <returns>Message response.</returns>
    [Description(WorkflowConstants.TextCompletion.CompleteStreamDescription)]
    [SKName(WorkflowConstants.TextCompletion.CompleteStreamName)]
    [SKFunction]
    public async Task<string> CompleteStreamAsync(SKContext context)
    {
        var reply = string.Empty;
        try
        {
            _completeHistory.Add(new ChatMessage(AuthorRole.User, context.Result));
            var previousText = GenerateContextString();
            var response = _textCompletion.CompleteStreamAsync(previousText, _completeRequestSettings, context.CancellationToken);

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

            reply = reply.Replace(previousText, string.Empty).Trim();

            // If the response is empty, remove the last sent message.
            if (string.IsNullOrEmpty(reply))
            {
                _completeHistory.RemoveAt(_completeHistory.Count - 1);
            }
            else
            {
                _completeHistory.Add(new ChatMessage(AuthorRole.Assistant, reply));
            }
        }
        catch (AIException e)
        {
            _logger.LogError(e, "Text Completion skill error.");
            _completeHistory.Remove(_completeHistory.LastOrDefault(p => p.Role == AuthorRole.User));
            var retried = false;
            if (_autoRemoveEarlierMessage
                && e.ErrorCode == AIException.ErrorCodes.InvalidRequest
                && e.Detail.TryGetTokenLimit(out var maxTokens, out var msgTokens))
            {
                _logger.LogInformation("Older messages have been removed, retrying");
                var canRetry = StaticHelpers.TryRemoveEarlierMessage(_completeHistory, maxTokens, msgTokens);
                if (canRetry)
                {
                    retried = true;
                    reply = await CompleteStreamAsync(context);
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
        _completeHistory.Clear();
        _completeHistory.Add(new ChatMessage(AuthorRole.System, prompt));
    }

    /// <summary>
    /// Set up chat history.
    /// </summary>
    /// <param name="messages">Chat history.</param>
    public void SetHistory(IEnumerable<Message> messages)
    {
        _completeHistory?.Clear();
        if (messages == null || _completeHistory == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(_systemPrompt))
        {
            _completeHistory.Add(new ChatMessage(AuthorRole.System, _systemPrompt));
        }

        foreach (var message in messages)
        {
            if (message.IsUser)
            {
                _completeHistory.Add(new ChatMessage(AuthorRole.User, message.Content));
            }
            else
            {
                _completeHistory.Add(new ChatMessage(AuthorRole.Assistant, message.Content));
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

            var textCompletion = kernel.GetService<ITextCompletion>();

            _textCompletion = textCompletion;
        }
    }

    private string GenerateContextString()
    {
        if (_completeHistory == null || _completeHistory.Count == 0)
        {
            return string.Empty;
        }

        var textList = new List<string>();
        foreach (var item in _completeHistory)
        {
            var role = string.Empty;
            if (item.Role == AuthorRole.User)
            {
                role = AppConstants.UserTag;
            }
            else if (item.Role == AuthorRole.Assistant)
            {
                role = AppConstants.AssistantTag;
            }
            else if (item.Role == AuthorRole.System)
            {
                role = AppConstants.SystemTag;
            }
            else
            {
                throw new NotSupportedException();
            }

            if (string.IsNullOrEmpty(item.Content))
            {
                continue;
            }

            textList.Add($"{role}: {item.Content}");
        }

        var text = string.Join("\n\n", textList);
        text += $"\n\n{AppConstants.AssistantTag}: ";
        return text;
    }

    private sealed class ChatMessage : ChatMessageBase
    {
        public ChatMessage(AuthorRole authorRole, string content)
            : base(authorRole, content)
        {
        }
    }
}
