// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SkillDefinition;
using RichasyAssistant.Libs.NativeSkills;
using RichasyAssistant.Services.Interfaces;

namespace RichasyAssistant.Services;

/// <summary>
/// Chat service.
/// </summary>
public sealed partial class ChatService
{
    private readonly IKernelService _kernelService;

    private IKernel _kernel;
    private IDictionary<string, ISKFunction> _chatFunctions;
    private IDictionary<string, ISKFunction> _completeFunctions;
    private ChatSkill _chatSkill;
    private TextCompleteSkill _completeSkill;
}
