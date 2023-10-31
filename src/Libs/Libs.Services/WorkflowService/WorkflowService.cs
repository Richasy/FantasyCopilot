// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Planning.Sequential;
using Microsoft.SemanticKernel.SkillDefinition;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Libs.NativeSkills;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Workspace;
using RichasyAssistant.Models.App.Workspace.Steps;
using RichasyAssistant.Models.Constants;
using Windows.ApplicationModel;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Workflow service.
/// </summary>
public sealed partial class WorkflowService : ServiceBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkflowService"/> class.
    /// </summary>
    private WorkflowService()
    {
        _kernel = Locator.Current.GetVariable<IKernel>();
        Locator.Current.VariableChanged += OnVariableChanged;
        _workflowContext = Locator.Current.GetVariable<WorkflowContext>();
        Initialize();
    }

    /// <summary>
    /// Execute workflow.
    /// </summary>
    /// <param name="input">Input text.</param>
    /// <param name="steps">Steps.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Execute result.</returns>
    public async Task<bool> ExecuteWorkflowAsync(string input, IEnumerable<WorkflowStep> steps, CancellationToken cancellationToken)
    {
        Logger.Debug($"To start executing the workflow, the input is {input ?? "None"}");
        _workflowContext.StepResults.Clear();
        _workflowContext.StepParameters.Clear();
        var list = new List<ISKFunction>();
        var semanticList = await GetSemanticFunctionsFromStepsAsync(steps);
        Logger.Debug($"Number of semantic skills acquired: {semanticList?.Count}");

        if (_pluginFunctions == null)
        {
            await ReloadPluginsAsync();
        }

        var contextVariables = new ContextVariables(input);
        foreach (var step in steps)
        {
            ISKFunction func = null;
            if (step.Skill == SkillType.Semantic)
            {
                ThrowIfSemanticNotSupported();
                var semanticTag = JsonSerializer.Deserialize<SemanticStep>(step.Detail);
                func = semanticList.First(p => p.Name == semanticTag.Id);
            }
            else if (step.Skill == SkillType.PluginCommand)
            {
                func = _pluginFunctions[step.PluginCommandId];
            }
            else
            {
                func = GetNativeFunctionNameFromSkill(step.Skill);
            }

            list.Add(func);
            _workflowContext.StepParameters.Add(step.Index, step.Detail);
            if (step.Skill == SkillType.PluginCommand && !string.IsNullOrEmpty(step.Detail))
            {
                var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(step.Detail);
                foreach (var p in parameters)
                {
                    contextVariables.Set(p.Key, p.Value);
                }
            }
        }

        var isError = false;
        try
        {
            var result = await RunWorkflowAsync(contextVariables, cancellationToken, list.ToArray());
            _workflowContext.StepResults.Add(_workflowContext.CurrentStepIndex, result.Result);
        }
        catch (Exception ex)
        {
            _kernel.LoggerFactory.CreateLogger<WorkflowService>().LogError(
                    ex,
                    "Something went wrong in pipeline step {0}:'{1}'",
                    _workflowContext.CurrentStepIndex,
                    ex.Message);
            isError = true;
            _workflowContext.StepResults.Add(WorkflowConstants.ErrorKey, ex.Message);
        }

        _workflowContext.RaiseResultUpdated();
        Logger.Debug("Workflow execution ended.");

        return !isError;
    }

    /// <summary>
    /// Try to get workflow steps based on goal.
    /// </summary>
    /// <param name="goal">Task goal.</param>
    /// <returns>Steps.</returns>
    public async Task<IEnumerable<WorkflowStep>> GetWorkflowStepsFromGoalAsync(string goal)
    {
        var localSemanticSkills = await _cacheToolkit.GetSemanticSkillsAsync();
        foreach (var config in localSemanticSkills)
        {
            RegisterFunctionFromSemanticSkill(config);
        }

        if (_pluginFunctions == null)
        {
            await ReloadPluginsAsync();
        }

        if (string.IsNullOrEmpty(_plannerPrompt))
        {
            var folder = Package.Current.InstalledPath;
            var promptFile = Path.Combine(folder, "Assets/plannerPrompt.txt");
            _plannerPrompt = await File.ReadAllTextAsync(promptFile);
        }

        var plannerConfig = new SequentialPlannerConfig();
        plannerConfig.ExcludedFunctions.Add(WorkflowConstants.Chat.InitializeName);
        plannerConfig.ExcludedFunctions.Add(WorkflowConstants.Chat.SendName);
        plannerConfig.ExcludedFunctions.Add(WorkflowConstants.TextCompletion.InitializeName);
        plannerConfig.ExcludedFunctions.Add(WorkflowConstants.TextCompletion.CompleteName);
        var planner = new SequentialPlanner(_kernel, plannerConfig, _plannerPrompt);
        var plan = await planner.CreatePlanAsync(goal);

        var context = _kernel.CreateNewContext();
        var functionsView = context.Skills.GetFunctionsView();

        var availableFunctions = functionsView.SemanticFunctions
            .Concat(functionsView.NativeFunctions)
            .SelectMany(x => x.Value)
            .ToList();
        var steps = new List<WorkflowStep>();
        foreach (var step in plan.Steps)
        {
            var data = new WorkflowStep();
            if (step.IsSemantic)
            {
                var semanticStep = new SemanticStep
                {
                    Id = step.Name,
                };
                data.Detail = JsonSerializer.Serialize(semanticStep);
                data.Skill = SkillType.Semantic;
            }
            else
            {
                var skill = GetSkillFromFunctionName(step.Name);
                if (skill == SkillType.None)
                {
                    var isGuid = Guid.TryParse(step.Name, out var guid);
                    if (!isGuid)
                    {
                        continue;
                    }
                }

                data.Skill = skill == SkillType.None ? SkillType.PluginCommand : skill;
                if (data.Skill == SkillType.PluginCommand)
                {
                    data.PluginCommandId = step.Name;
                }
            }

            steps.Add(data);
        }

        return steps;
    }

    /// <summary>
    /// Reload all plugins.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task ReloadPluginsAsync()
    {
        var plugins = await _cacheToolkit.GetPluginConfigsAsync(true);
        var functions = _kernel.Skills.GetFunctionsView(false, true);
        if (functions.NativeFunctions.Any())
        {
            functions.NativeFunctions.First().Value.RemoveAll(p => Guid.TryParse(p.Name, out var _));
        }

        if (_pluginFunctions == null)
        {
            _pluginFunctions = new Dictionary<string, ISKFunction>();
        }
        else
        {
            _pluginFunctions.Clear();
        }

        if (plugins.Any())
        {
            foreach (var plugin in plugins)
            {
                if (plugin.Commands?.Any() ?? false)
                {
                    plugin.Commands
                        .Select(p => new CommandFunction(p, plugin.Id))
                        .ToList()
                        .ForEach(f =>
                        {
                            var newF = _kernel.RegisterCustomFunction(f);
                            _pluginFunctions.Add(newF.Name, newF);
                        });
                }
            }
        }

        Logger.Debug($"The plugin has been overloaded, the number of plugin methods: {_pluginFunctions.Count}");
    }

    private static void ThrowIfSemanticNotSupported()
    {
        if (!KernelService.Instance.IsChatSupport)
        {
            throw new System.InvalidOperationException("Semantic is not supported.");
        }
    }

    private void Initialize()
    {
        _kernel.ImportSkill(new PlaceholderSkill());
        _textFunctions = _kernel.ImportSkill(new TextSkill());
        _voiceFunctions = _kernel.ImportSkill(new VoiceSkill());
        _imageFunctions = _kernel.ImportSkill(new ImageSkill());
        _knowledgeFunctions = _kernel.ImportSkill(new KnowledgeSkill());
        _nativeFunctions = _kernel.ImportSkill(new NativeSkill());
    }

    private void OnVariableChanged(object sender, Type e)
    {
        if (e == typeof(IKernel))
        {
            _kernel = Locator.Current.GetVariable<IKernel>();
            Initialize();
            _pluginFunctions = default;
        }
    }
}
