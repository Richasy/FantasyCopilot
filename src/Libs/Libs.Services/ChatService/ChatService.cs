// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Libs.NativeSkills;
using RichasyAssistant.Models.App.Gpt;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Chat service.
/// </summary>
public sealed partial class ChatService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatService"/> class.
    /// </summary>
    private ChatService()
    {
        Locator.Current.VariableChanged += OnVariableChanged;
        _kernel = Locator.Current.GetVariable<IKernel>();
    }

    /// <summary>
    /// Occurs when new content is received while waiting for an LLM response.
    /// </summary>
    public event EventHandler<string> CharacterReceived;

    /// <summary>
    /// Create a new chat.
    /// </summary>
    /// <param name="systemPrompt">System prompt.</param>
    public void CreateNewChat(string systemPrompt)
    {
        ThrowIfNotSupportChat();
        if (_chatSkill == null && KernelService.Instance.HasChatModel)
        {
            _chatSkill = new ChatSkill();
            _chatSkill.CharacterReceived += (s, e) => CharacterReceived?.Invoke(this, e);
            _chatFunctions = _kernel.ImportSkill(_chatSkill);
        }

        if (_completeSkill == null && KernelService.Instance.HasTextCompletionModel)
        {
            _completeSkill = new TextCompleteSkill();
            _completeSkill.CharacterReceived += (s, e) => CharacterReceived?.Invoke(this, e);
            _completeFunctions = _kernel.ImportSkill(_completeSkill);
        }

        if (KernelService.Instance.HasChatModel)
        {
            _chatSkill.Create(systemPrompt);
        }
        else
        {
            _completeSkill?.Create(systemPrompt);
        }
    }

    /// <summary>
    /// Get chat message response.
    /// </summary>
    /// <param name="message">New message.</param>
    /// <param name="options">Session options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="MessageResponse"/>.</returns>
    public async Task<MessageResponse> GetMessageResponseAsync(Message message, SessionOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotSupportChat();
        var pipelines = KernelService.Instance.HasChatModel
            ? options.UseStreamOutput
                ? new List<ISKFunction> { _chatFunctions[WorkflowConstants.Chat.InitializeName], _chatFunctions[WorkflowConstants.Chat.GenerateStreamName] }
                : new List<ISKFunction> { _chatFunctions[WorkflowConstants.Chat.InitializeName], _chatFunctions[WorkflowConstants.Chat.SendName] }
            : options.UseStreamOutput
                ? new List<ISKFunction> { _completeFunctions[WorkflowConstants.TextCompletion.InitializeName], _completeFunctions[WorkflowConstants.TextCompletion.CompleteStreamName] }
                : new List<ISKFunction> { _completeFunctions[WorkflowConstants.TextCompletion.InitializeName], _completeFunctions[WorkflowConstants.TextCompletion.CompleteName] };

        var contextVariables = new ContextVariables(message.Content);
        contextVariables.Set(AppConstants.SessionOptionsKey, JsonSerializer.Serialize(options));
        try
        {
            var result = await _kernel.RunAsync(contextVariables, cancellationToken, pipelines.ToArray());
            return result.Result == null
                ? new MessageResponse(true, "No response.")
                : result.Result.StartsWith(AppConstants.ExceptionTag)
                    ? new MessageResponse(true, result.Result.Replace(AppConstants.ExceptionTag, string.Empty))
                    : new MessageResponse(false, result.Result);
        }
        catch (Exception ex)
        {
            return new MessageResponse(true, ex.Message);
        }
    }

    /// <summary>
    /// Update chat history.
    /// </summary>
    /// <param name="history">Chat history.</param>
    public void SetHistory(IEnumerable<Message> history)
        => _chatSkill?.SetHistory(history);

    private static void ThrowIfNotSupportChat()
    {
        if (!KernelService.Instance.IsChatSupport)
        {
            throw new System.InvalidOperationException("Chat is not supported.");
        }
    }

    private void OnVariableChanged(object sender, Type e)
    {
        if (e == typeof(IKernel))
        {
            _kernel = Locator.Current.GetVariable<IKernel>();
            if (_chatSkill != null)
            {
                _chatFunctions = _kernel.ImportSkill(_chatSkill);
            }

            if (_completeSkill != null)
            {
                _completeFunctions = _kernel.ImportSkill(_completeSkill);
            }
        }
    }
}
