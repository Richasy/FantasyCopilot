// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Models.App.Knowledge;
using FantasyCopilot.Models.App.Workspace.Steps;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace FantasyCopilot.Libs.NativeSkills;

/// <summary>
/// Knowledge skill.
/// </summary>
public sealed class KnowledgeSkill
{
    private readonly ICacheToolkit _cacheToolkit;
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IMemoryService _memoryService;
    private readonly WorkflowContext _workflowContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgeSkill"/> class.
    /// </summary>
    public KnowledgeSkill()
    {
        _cacheToolkit = Locator.Current.GetService<ICacheToolkit>();
        _settingsToolkit = Locator.Current.GetService<ISettingsToolkit>();
        _memoryService = Locator.Current.GetService<IMemoryService>();
        _workflowContext = Locator.Current.GetVariable<WorkflowContext>();
    }

    /// <summary>
    /// Get knowledge from base.
    /// </summary>
    /// <param name="context">Current context.</param>
    /// <returns>Knowledge text.</returns>
    [SKName(WorkflowConstants.Knowledge.GetKnowledgeName)]
    [Description(WorkflowConstants.Knowledge.GetKnowledgeDescription)]
    public async Task<string> GetKnowledgeAsync(SKContext context)
    {
        var @base = await TryConnectKnowledgeBaseAsync(context);
        if (@base == null)
        {
            _memoryService.DisconnectSQLiteKnowledgeBase();
            return default;
        }

        var sessionOptions = new SessionOptions
        {
            FrequencyPenalty = 1,
            PresencePenalty = 1,
            MaxResponseTokens = _settingsToolkit.ReadLocalSetting(SettingNames.DefaultMaxResponseTokens, 400),
            Temperature = _settingsToolkit.ReadLocalSetting(SettingNames.DefaultTemperature, 0.4),
            TopP = _settingsToolkit.ReadLocalSetting(SettingNames.DefaultTopP, 0),
        };
        var answer = await _memoryService.QuickSearchMemoryAsync(context.Result, sessionOptions, context.CancellationToken);
        if (answer.IsError)
        {
            context.Fail(answer.Content);
            return default;
        }

        var sourceKey = string.Format(WorkflowConstants.KnowledgeSourceKey, _workflowContext.CurrentStepIndex);
        context.Variables.Set(sourceKey, answer.AdditionalContent);

        _memoryService.DisconnectSQLiteKnowledgeBase();
        return answer.Content;
    }

    /// <summary>
    /// Import file to knowledge base.
    /// </summary>
    /// <param name="context">Current context.</param>
    /// <returns>Previous result.</returns>
    [SKName(WorkflowConstants.Knowledge.ImportFileName)]
    [Description(WorkflowConstants.Knowledge.ImportFileDescription)]
    public async Task<string> ImportFileAsync(SKContext context)
    {
        var path = context.Result;
        if (!File.Exists(path))
        {
            _memoryService.DisconnectSQLiteKnowledgeBase();
            context.Fail("File not found.");
            return default;
        }

        var @base = await TryConnectKnowledgeBaseAsync(context);
        if (@base == null)
        {
            _memoryService.DisconnectSQLiteKnowledgeBase();
            return default;
        }

        var isImported = await _memoryService.ImportFileToMemoryAsync(path);
        if (!isImported)
        {
            _memoryService.DisconnectSQLiteKnowledgeBase();
            context.Fail("Can not import file.");
            return default;
        }

        _memoryService.DisconnectSQLiteKnowledgeBase();
        return context.Result;
    }

    /// <summary>
    /// Import folder to knowledge base.
    /// </summary>
    /// <param name="context">Current context.</param>
    /// <returns>Previous result.</returns>
    [SKName(WorkflowConstants.Knowledge.ImportFolderName)]
    [Description(WorkflowConstants.Knowledge.ImportFolderDescription)]
    public async Task<string> ImportFolderAsync(SKContext context)
    {
        var parameters = _workflowContext.GetStepParameters<KnowledgeBaseStep>();
        if (string.IsNullOrEmpty(parameters.FileSearchPattern))
        {
            parameters.FileSearchPattern = "*.*";
        }

        var path = context.Result;
        if (!Directory.Exists(path))
        {
            context.Fail("Folder not found.");
            return default;
        }

        var @base = await TryConnectKnowledgeBaseAsync(context);
        if (@base == null)
        {
            _memoryService.DisconnectSQLiteKnowledgeBase();
            return default;
        }

        var (total, failed) = await _memoryService.ImportFolderToMemoryAsync(path, parameters.FileSearchPattern);
        if (total == failed)
        {
            _memoryService.DisconnectSQLiteKnowledgeBase();
            context.Fail("Can not import folder.");
            return default;
        }

        _memoryService.DisconnectSQLiteKnowledgeBase();
        return context.Result;
    }

    private async Task<KnowledgeBase> TryConnectKnowledgeBaseAsync(SKContext context)
    {
        var parameters = _workflowContext.GetStepParameters<KnowledgeBaseStep>();
        if (parameters == null)
        {
            context.Fail("Do not have knowledge base parameters");
            return default;
        }

        var input = context.Result;
        if (string.IsNullOrEmpty(input))
        {
            context.Fail("Input is empty.");
            return default;
        }

        var config = (await _cacheToolkit.GetKnowledgeBasesAsync()).FirstOrDefault(p => p.Id == parameters.KnowledgeBaseId);
        if (config == null)
        {
            context.Fail("Knowledge base not found.");
            return default;
        }

        var isConnected = await _memoryService.ConnectSQLiteKnowledgeBaseAsync(config.DatabasePath);
        if (!isConnected)
        {
            context.Fail("Can not connect to database.");
            return default;
        }

        return config;
    }
}
