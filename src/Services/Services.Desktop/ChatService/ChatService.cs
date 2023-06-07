// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Libs.NativeSkills;
using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace FantasyCopilot.Services;

/// <summary>
/// Chat service.
/// </summary>
public sealed partial class ChatService : IChatService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatService"/> class.
    /// </summary>
    public ChatService(IKernelService kernelService)
    {
        _kernelService = kernelService;
        Locator.Current.VariableChanged += OnVariableChanged;
        _kernel = Locator.Current.GetVariable<IKernel>();
    }

    /// <inheritdoc/>
    public void CreateNewChat(string systemPrompt)
    {
        ThrowIfNotSupportChat();
        if (_chatSkill == null && _kernelService.HasChatModel)
        {
            _chatSkill = new ChatSkill();
            _chatFunctions = _kernel.ImportSkill(_chatSkill);
        }

        if (_completeSkill == null && !_kernelService.HasChatModel)
        {
            _completeSkill = new TextCompleteSkill();
            _completeFunctions = _kernel.ImportSkill(_completeSkill);
        }

        if (_kernelService.HasChatModel)
        {
            _chatSkill.Create(systemPrompt);
        }
        else
        {
            _completeSkill?.Create(systemPrompt);
        }
    }

    /// <inheritdoc/>
    public async Task<MessageResponse> GetMessageResponseAsync(Message message, SessionOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotSupportChat();
        var pipelines = _kernelService.HasChatModel
            ? new List<ISKFunction> { _chatFunctions[WorkflowConstants.Chat.InitializeName], _chatFunctions[WorkflowConstants.Chat.SendName] }
            : new List<ISKFunction> { _completeFunctions[WorkflowConstants.TextCompletion.InitializeName], _completeFunctions[WorkflowConstants.TextCompletion.CompleteName] };

        var contextVariables = new ContextVariables(message.Content);
        contextVariables.Set(AppConstants.SessionOptionsKey, JsonSerializer.Serialize(options));
        var result = await _kernel.RunAsync(contextVariables, cancellationToken, pipelines.ToArray());
        return result.ErrorOccurred
            ? new MessageResponse(true, result.LastErrorDescription)
            : result.Result == null
                ? new MessageResponse(true, "No response.")
                : result.Result.StartsWith(AppConstants.ExceptionTag)
                    ? new MessageResponse(true, result.Result.Replace(AppConstants.ExceptionTag, string.Empty))
                    : new MessageResponse(false, result.Result);
    }

    /// <inheritdoc/>
    public void SetHistory(IEnumerable<Message> history)
        => _chatSkill?.SetHistory(history);

    private void ThrowIfNotSupportChat()
    {
        if (!_kernelService.IsChatSupport)
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
