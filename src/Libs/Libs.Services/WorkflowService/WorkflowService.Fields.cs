// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SkillDefinition;
using RichasyAssistant.Models.App;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Workflow service.
/// </summary>
public sealed partial class WorkflowService
{
    private readonly ICacheToolkit _cacheToolkit;
    private readonly WorkflowContext _workflowContext;

    private IKernel _kernel;
    private string _plannerPrompt;

    private IDictionary<string, ISKFunction> _textFunctions;
    private IDictionary<string, ISKFunction> _voiceFunctions;
    private IDictionary<string, ISKFunction> _imageFunctions;
    private IDictionary<string, ISKFunction> _knowledgeFunctions;
    private IDictionary<string, ISKFunction> _pluginFunctions;
    private IDictionary<string, ISKFunction> _nativeFunctions;

    /// <summary>
    /// Instance.
    /// </summary>
    public static WorkflowService Instance { get; } = new WorkflowService();
}
