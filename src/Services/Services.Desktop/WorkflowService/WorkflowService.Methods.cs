// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.Models.App.Workspace.Steps;
using FantasyCopilot.Models.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;

namespace FantasyCopilot.Services;

/// <summary>
/// Workflow service.
/// </summary>
public sealed partial class WorkflowService
{
    private static SkillType GetSkillFromFunctionName(string functionName)
    {
        return functionName switch
        {
            WorkflowConstants.Text.TranslateName => SkillType.Translate,
            WorkflowConstants.Text.OverwriteName => SkillType.TextOverwrite,
            WorkflowConstants.Text.VariableRedirectName => SkillType.VariableRedirect,
            WorkflowConstants.Text.VariableCreateName => SkillType.VariableCreate,
            WorkflowConstants.Voice.ReadTextName => SkillType.TextToSpeech,
            WorkflowConstants.Image.DrawName => SkillType.TextToImage,
            WorkflowConstants.Knowledge.GetKnowledgeName => SkillType.GetKnowledge,
            WorkflowConstants.Knowledge.ImportFileName => SkillType.ImportFileToKnowledge,
            WorkflowConstants.Knowledge.ImportFolderName => SkillType.ImportFolderToKnowledge,
            WorkflowConstants.Native.TextNotificationName => SkillType.TextNotification,
            WorkflowConstants.Input.TextName => SkillType.InputText,
            WorkflowConstants.Input.VoiceName => SkillType.InputVoice,
            WorkflowConstants.Input.FileName => SkillType.InputFile,
            WorkflowConstants.Output.TextName => SkillType.OutputText,
            WorkflowConstants.Output.VoiceName => SkillType.OutputVoice,
            WorkflowConstants.Output.ImageName => SkillType.OutputImage,
            _ => SkillType.None,
        };
    }

    private ISKFunction GetNativeFunctionNameFromSkill(SkillType skill)
    {
        return skill switch
        {
            SkillType.Translate => _textFunctions[WorkflowConstants.Text.TranslateName],
            SkillType.TextOverwrite => _textFunctions[WorkflowConstants.Text.OverwriteName],
            SkillType.VariableRedirect => _textFunctions[WorkflowConstants.Text.VariableRedirectName],
            SkillType.VariableCreate => _textFunctions[WorkflowConstants.Text.VariableCreateName],
            SkillType.TextToSpeech => _voiceFunctions[WorkflowConstants.Voice.ReadTextName],
            SkillType.TextToImage => _imageFunctions[WorkflowConstants.Image.DrawName],
            SkillType.GetKnowledge => _knowledgeFunctions[WorkflowConstants.Knowledge.GetKnowledgeName],
            SkillType.ImportFileToKnowledge => _knowledgeFunctions[WorkflowConstants.Knowledge.ImportFileName],
            SkillType.ImportFolderToKnowledge => _knowledgeFunctions[WorkflowConstants.Knowledge.ImportFolderName],
            SkillType.TextNotification => _nativeFunctions[WorkflowConstants.Native.TextNotificationName],
            _ => throw new NotImplementedException(),
        };
    }

    private async Task<List<ISKFunction>> GetSemanticFunctionsFromStepsAsync(IEnumerable<WorkflowStep> steps)
    {
        var list = new List<ISKFunction>();
        var semanticFunctions = steps.Where(p => p.Skill == SkillType.Semantic)
            .Select(p => JsonSerializer.Deserialize<SemanticStep>(p.Detail))
            .DistinctBy(p => p.Id)
            .ToList();
        if (semanticFunctions.Any())
        {
            foreach (var item in semanticFunctions)
            {
                var config = await _cacheToolkit.GetSemanticSkillByIdAsync(item.Id)
                    ?? throw new Exception("Invalid semantic skill id");

                var function = RegisterFunctionFromSemanticSkill(config);
                list.Add(function);
            }
        }

        return list;
    }

    private ISKFunction RegisterFunctionFromSemanticSkill(SemanticSkillConfig config)
    {
        var promptConfig = new PromptTemplateConfig
        {
            Description = config.Description,
            Completion =
            {
                MaxTokens = config.MaxResponseTokens,
                FrequencyPenalty = config.FrequencyPenalty,
                PresencePenalty = config.PresencePenalty,
                Temperature = config.Temperature,
                TopP = config.TopP,
            },
        };

        var promptTemplate = new PromptTemplate(config.Prompt, promptConfig, _kernel);
        var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);
        var function = _kernel.RegisterSemanticFunction(config.Id, functionConfig);
        return function;
    }

    private async Task<SKContext> RunWorkflowAsync(ContextVariables variables, CancellationToken cancellationToken, params ISKFunction[] pipeline)
    {
        var context = new SKContext(variables, _kernel.Skills, _kernel.LoggerFactory);
        _workflowContext.CurrentStepIndex = -1;
        context.Variables.Set(WorkflowConstants.OriginalKey, variables.Input);
        foreach (var f in pipeline)
        {
            if (_workflowContext.CurrentStepIndex >= 0)
            {
                _workflowContext.StepResults.Add(_workflowContext.CurrentStepIndex, context.Result);
                context.Variables.Set(string.Format(WorkflowConstants.StepResultKey, _workflowContext.CurrentStepIndex), context.Result);
                _workflowContext.RaiseResultUpdated();
            }

            _workflowContext.CurrentStepIndex++;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                context = await f.InvokeAsync(context, default, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _kernel.LoggerFactory.CreateLogger<WorkflowService>().LogError(
                    e,
                    "Something went wrong in pipeline step {0}: {1}.{2}. Error: {3}",
                    _workflowContext.CurrentStepIndex,
                    f.SkillName,
                    f.Name,
                    e.Message);
                throw new SKException(e.Message, e);
            }
        }

        return context;
    }
}
