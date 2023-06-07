// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.Generic;
using FantasyCopilot.Libs.NativeSkills;
using FantasyCopilot.Services.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SkillDefinition;

namespace FantasyCopilot.Services;

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
