// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.Generic;
using FantasyCopilot.Models.App;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SkillDefinition;

namespace FantasyCopilot.Services;

/// <summary>
/// Workflow service.
/// </summary>
public sealed partial class WorkflowService
{
    private readonly ICacheToolkit _cacheToolkit;
    private readonly IKernelService _kernelService;
    private readonly ILogger<WorkflowService> _logger;
    private readonly WorkflowContext _workflowContext;

    private IKernel _kernel;
    private string _plannerPrompt;

    private IDictionary<string, ISKFunction> _textFunctions;
    private IDictionary<string, ISKFunction> _voiceFunctions;
    private IDictionary<string, ISKFunction> _imageFunctions;
    private IDictionary<string, ISKFunction> _knowledgeFunctions;
    private IDictionary<string, ISKFunction> _pluginFunctions;
    private IDictionary<string, ISKFunction> _nativeFunctions;
}
