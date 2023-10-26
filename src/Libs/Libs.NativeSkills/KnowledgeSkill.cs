// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Gpt;
using RichasyAssistant.Models.App.Knowledge;
using RichasyAssistant.Models.App.Workspace.Steps;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.Libs.NativeSkills;

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
    [SKFunction]
    public async Task<string> GetKnowledgeAsync(SKContext context, CancellationToken cancellationToken)
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
        var answer = await _memoryService.QuickSearchMemoryAsync(context.Result, sessionOptions, cancellationToken);
        if (answer.IsError)
        {
            throw new SKException(answer.Content);
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
    [SKFunction]
    public async Task<string> ImportFileAsync(SKContext context)
    {
        var path = context.Result;
        if (!File.Exists(path))
        {
            _memoryService.DisconnectSQLiteKnowledgeBase();
            throw new SKException("File not found.");
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
            throw new SKException("Can not import file.");
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
    [SKFunction]
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
            throw new SKException("Folder not found.");
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
            throw new SKException("Can not import folder.");
        }

        _memoryService.DisconnectSQLiteKnowledgeBase();
        return context.Result;
    }

    private async Task<KnowledgeBase> TryConnectKnowledgeBaseAsync(SKContext context)
    {
        var parameters = _workflowContext.GetStepParameters<KnowledgeBaseStep>()
            ?? throw new SKException("Do not have knowledge base parameters");

        var input = context.Result;
        if (string.IsNullOrEmpty(input))
        {
            throw new SKException("Input is empty.");
        }

        var config = (await _cacheToolkit.GetKnowledgeBasesAsync()).FirstOrDefault(p => p.Id == parameters.KnowledgeBaseId)
            ?? throw new SKException("Knowledge base not found.");

        var isConnected = await _memoryService.ConnectSQLiteKnowledgeBaseAsync(config.DatabasePath);
        return !isConnected ? throw new SKException("Can not connect to database.") : config;
    }
}
