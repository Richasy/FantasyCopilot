﻿// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.TemplateEngine.Prompt;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Workspace.Steps;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Services.Interfaces;

namespace RichasyAssistant.Libs.NativeSkills;

/// <summary>
/// Text skill.
/// </summary>
public sealed class TextSkill
{
    private readonly ITranslateService _translateService;
    private readonly WorkflowContext _workflowContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextSkill"/> class.
    /// </summary>
    public TextSkill()
    {
        _workflowContext = Locator.Current.GetVariable<WorkflowContext>();
        _translateService = Locator.Current.GetService<ITranslateService>();
    }

    /// <summary>
    /// Translate the text into the specified language.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Translated text.</returns>
    [Description(WorkflowConstants.Text.TranslateDescription)]
    [SKName(WorkflowConstants.Text.TranslateName)]
    [SKFunction]
    public async Task<string> TranslateAsync(SKContext context, CancellationToken cancellationToken)
    {
        var parameters = _workflowContext.GetStepParameters<TranslateStep>()
            ?? throw new SKException("Do not have translate parameters");

        var text = context.Result;
        if (string.IsNullOrEmpty(text))
        {
            throw new SKException("The text content to be translated cannot be empty");
        }

        var result = await _translateService.TranslateTextAsync(text, parameters.Source, parameters.Target, cancellationToken);
        return result;
    }

    /// <summary>
    /// Overwrite input text, output as new text.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>New text.</returns>
    [SKName(WorkflowConstants.Text.OverwriteName)]
    [Description(WorkflowConstants.Text.OverwriteDescription)]
    [SKFunction]
    public async Task<string> TextOverwriteAsync(SKContext context)
    {
        var parameters = _workflowContext.GetStepParameters<TextOverwriteStep>()
            ?? throw new SKException("Do not have overwrite parameters");

        var templateEngine = new PromptTemplateEngine();
        var finalResult = await templateEngine.RenderAsync(parameters.Text, context);
        return finalResult;
    }

    /// <summary>
    /// Copy the value of one variable in the context to another variable.
    /// </summary>
    /// <param name="context">Current context.</param>
    /// <returns><see cref="Task"/>.</returns>
    [SKName(WorkflowConstants.Text.VariableRedirectName)]
    [Description(WorkflowConstants.Text.VariableRedirectDescription)]
    [SKFunction]
    public Task<SKContext> VariableRedirectAsync(SKContext context)
    {
        var parameters = _workflowContext.GetStepParameters<VariableRedirectStep>();
        if (parameters == null
            || string.IsNullOrEmpty(parameters.SourceName)
            || string.IsNullOrEmpty(parameters.TargetName))
        {
            throw new SKException("Do not have variable redirect parameters");
        }

        var hasSource = context.Variables.TryGetValue(parameters.SourceName, out var value);
        if (!hasSource)
        {
            throw new SKException("Do not have source variable.");
        }

        context.Variables.Set(parameters.TargetName, value);
        return Task.FromResult(context);
    }

    /// <summary>
    /// Create a new variable.
    /// </summary>
    /// <param name="context">Current context.</param>
    /// <returns><see cref="Task"/>.</returns>
    [SKName(WorkflowConstants.Text.VariableCreateName)]
    [Description(WorkflowConstants.Text.VariableCreateDescription)]
    [SKFunction]
    public async Task<SKContext> VariableCreateAsync(SKContext context)
    {
        var parameters = _workflowContext.GetStepParameters<VariableCreateStep>();
        if (parameters == null
            || string.IsNullOrEmpty(parameters.Name)
            || string.IsNullOrEmpty(parameters.Value))
        {
            throw new SKException("Do not have variable create parameters");
        }

        var engine = new PromptTemplateEngine();
        var value = await engine.RenderAsync(parameters.Value, context);

        context.Variables.Set(parameters.Name, value);
        return context;
    }
}
