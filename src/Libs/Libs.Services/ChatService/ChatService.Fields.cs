// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SkillDefinition;
using RichasyAssistant.Libs.NativeSkills;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Chat service.
/// </summary>
public sealed partial class ChatService
{
    private IKernel _kernel;
    private IDictionary<string, ISKFunction> _chatFunctions;
    private IDictionary<string, ISKFunction> _completeFunctions;
    private ChatSkill _chatSkill;
    private TextCompleteSkill _completeSkill;

    /// <summary>
    /// Instance.
    /// </summary>
    public static ChatService Instance { get; } = new ChatService();
}
