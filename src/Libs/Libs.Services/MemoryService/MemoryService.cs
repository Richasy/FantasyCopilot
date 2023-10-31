// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Memory.Sqlite;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Text;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App.Gpt;
using RichasyAssistant.Models.App.Knowledge;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Memory service.
/// </summary>
public sealed partial class MemoryService : ServiceBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryService"/> class.
    /// </summary>
    public MemoryService()
    {
        _kernel = Locator.Current.GetVariable<IKernel>();
        Locator.Current.VariableChanged += OnVariableChanged;
    }

    /// <summary>
    /// Connect to the knowledge base.
    /// </summary>
    /// <param name="databasePath">SQLite database path.</param>
    /// <returns>Whether the connection was successful.</returns>
    public async Task<bool> ConnectSQLiteKnowledgeBaseAsync(string databasePath)
    {
        ThrowIfNotSupportMemory();
        if (!File.Exists(databasePath))
        {
            Logger.Error($"The database file corresponding to the knowledge base has been deleted or moved");
            return false;
        }

        try
        {
            DisconnectSQLiteKnowledgeBase();
            Logger.Debug($"Connecting to database: {databasePath}");
            var store = await SqliteMemoryStore.ConnectAsync(databasePath);
            _memoryStore = store;
            _kernel.UseMemory(store);
            Logger.Debug($"Fetching available collections...");
            var collections = await _kernel.Memory.GetCollectionsAsync();
            Logger.Debug($"Found {collections.Count} collections");
            _tempMemoryCollections = collections.ToList();
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "An error occurred while trying to connect to the database");
            DisconnectSQLiteKnowledgeBase();
            return false;
        }
    }

    /// <summary>
    /// Disconnect from the current knowledge base.
    /// </summary>
    public void DisconnectSQLiteKnowledgeBase()
    {
        try
        {
            if (_memoryStore != null)
            {
                Logger.Debug($"Closing database");
                _memoryStore?.Dispose();
                _memoryStore = null;
                Logger.Debug($"Database closed");
            }
        }
        catch (System.Exception ex)
        {
            Logger.Error(ex, "An error occurred while trying to close the database");
        }
    }

    /// <summary>
    /// Gets the progress of importing files into memory.
    /// </summary>
    /// <returns>imported count and total count.</returns>
    public (int ImportedFiles, int TotalFiles) GetImportToMemoryProgress()
        => (_tempMemoryFileImportedCount, _tempMemoryFileTotalCount);

    /// <summary>
    /// Import the contents of the file into memory.
    /// </summary>
    /// <param name="filePath">File path.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task<bool> ImportFileToMemoryAsync(string filePath)
    {
        ThrowIfNotSupportMemory();
        _tempMemoryFileTotalCount = 1;
        _tempMemoryFileFailedCount = 0;
        _tempMemoryFileImportedCount = 0;
        Logger.Info($"Importing file: {filePath}");
        await SaveContextInformationAsync(filePath);
        Logger.Info($"Import file finished");
        DisconnectSQLiteKnowledgeBase();
        return _tempMemoryFileFailedCount == 0;
    }

    /// <summary>
    /// Import folder to memory.
    /// </summary>
    /// <param name="folderPath">The root directory path.</param>
    /// <param name="searchPattern">Search expression.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task<(int TotalCount, int FailCount)> ImportFolderToMemoryAsync(string folderPath, string searchPattern)
    {
        ThrowIfNotSupportMemory();
        _tempMemoryFileTotalCount = 0;
        _tempMemoryFileFailedCount = 0;
        _tempMemoryFileImportedCount = 0;
        Logger.Info($"Importing folder: {folderPath}");
        var files = await GetFilesAsync(folderPath, searchPattern);
        _tempMemoryFileTotalCount = files.Count;
        var tasks = new List<Task>();
        foreach (var file in files)
        {
            Logger.Info($"Importing {file.FullName}");
            tasks.Add(SaveContextInformationAsync(file.FullName, folderPath));
        }

        await Task.WhenAll(tasks);
        Logger.Info($"Import folder finished");
        DisconnectSQLiteKnowledgeBase();
        return (_tempMemoryFileTotalCount, _tempMemoryFileFailedCount);
    }

    /// <summary>
    /// query memory (context lookup).
    /// </summary>
    /// <param name="query">Query text.</param>
    /// <param name="options">Session options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Message.</returns>
    public async Task<MessageResponse> QuickSearchMemoryAsync(string query, SessionOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotSupportMemory();
        var text = string.Empty;
        var id = string.Empty;
        var collectionId = _tempMemoryCollections.FirstOrDefault();
        var minRelevanceScore = _settingsToolkit.ReadLocalSetting(SettingNames.ContextMinRelevanceScore, 0.7d);
        await foreach (var item in _kernel.Memory.SearchAsync(collectionId ?? AppConstants.KnowledgeBaseCollectionId, query, minRelevanceScore: minRelevanceScore, cancellationToken: cancellationToken))
        {
            text = item.Metadata.Text;
            id = item.Metadata.AdditionalMetadata ?? item.Metadata.Id;
        }

        if (string.IsNullOrEmpty(text))
        {
            return new MessageResponse(false, "No answer");
        }

        var msg = await GetAnswerFromContextAsync(query, new[] { new KnowledgeContext { FileName = id, Content = text } }, options, cancellationToken);
        return msg;
    }

    /// <summary>
    /// Search the positioning context from the database.
    /// </summary>
    /// <param name="query">Query text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Knowledge context list.</returns>
    public async Task<IEnumerable<KnowledgeContext>> AdvancedSearchMemoryAsync(string query, CancellationToken cancellationToken)
    {
        ThrowIfNotSupportMemory();
        var collectionId = _tempMemoryCollections.FirstOrDefault();
        var contextList = new List<KnowledgeContext>();
        var contextLimit = _settingsToolkit.ReadLocalSetting(SettingNames.ContextLimit, 3);
        var minRelevanceScore = _settingsToolkit.ReadLocalSetting(SettingNames.ContextMinRelevanceScore, 0.7d);
        await foreach (var item in _kernel.Memory.SearchAsync(collectionId ?? AppConstants.KnowledgeBaseCollectionId, query, contextLimit, minRelevanceScore, cancellationToken: cancellationToken))
        {
            var context = new KnowledgeContext
            {
                FileName = item.Metadata.AdditionalMetadata ?? item.Metadata.Id,
                Content = item.Metadata.Text,
                Score = item.Relevance,
            };
            contextList.Add(context);
        }

        return contextList;
    }

    /// <summary>
    /// Pass in the relevant context, after comprehensive summary, provide answers according to the questions.
    /// </summary>
    /// <param name="query">Query text.</param>
    /// <param name="contextList">Knowledge context list.</param>
    /// <param name="options">Session options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Message.</returns>
    public async Task<MessageResponse> GetAnswerFromContextAsync(string query, IEnumerable<KnowledgeContext> contextList, SessionOptions options, CancellationToken cancellationToken)
    {
        var func = GetQAFunction(options);
        var variables = new ContextVariables(query);
        var text = string.Join("\n====\n", contextList.Select(x => x.Content));
        variables.Set("Content", text);

        var isError = false;
        try
        {
            var context = await _kernel.RunAsync(variables, func);
            text = context.Result.Trim();
            isError = string.IsNullOrEmpty(text);
        }
        catch (Exception)
        {
            isError = true;
        }

        var data = isError ? "Something error" : text;
        return new MessageResponse(isError, data, string.Join(" | ", contextList.Select(p => p.FileName)));
    }

    private static string BuildFileId(string filePath, string rootDirectory)
        => string.IsNullOrEmpty(rootDirectory)
            ? filePath.Replace('\\', '/')
            : Path.GetRelativePath(rootDirectory, filePath).Replace('\\', '/');

    private static void ThrowIfNotSupportMemory()
    {
        if (!KernelService.Instance.IsMemorySupport)
        {
            throw new System.InvalidOperationException("Memory is not supported.");
        }
    }

    private static async Task<List<FileInfo>> GetFilesAsync(string rootDirectory, string extensionList)
    {
        if (string.IsNullOrEmpty(extensionList))
        {
            throw new System.InvalidCastException("Extension list is empty.");
        }

        var sp = extensionList.Split(',');
        var directory = new DirectoryInfo(rootDirectory);
        var files = new List<FileInfo>();
        await Task.Run(() =>
        {
            files = sp
                .SelectMany(p => directory.GetFiles(p.Trim(), SearchOption.AllDirectories))
                .ToList();
        });
        return files;
    }

    private async Task SaveContextInformationAsync(string filePath, string rootDirectory = default)
    {
        var fileId = BuildFileId(filePath, rootDirectory);

        try
        {
            var maxContentLength = _settingsToolkit.ReadLocalSetting(SettingNames.MaxSplitContentLength, 1024);
            var maxTokenLength = _settingsToolkit.ReadLocalSetting(SettingNames.MaxParagraphTokenLength, 512);
            var content = await File.ReadAllTextAsync(filePath);
            var isMarkdown = Path.GetExtension(filePath).Equals(".md", StringComparison.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(content))
            {
                _tempMemoryFileFailedCount++;
                return;
            }

            var needSplit = content.Length > maxContentLength;
            if (needSplit)
            {
                List<string> lines;
                List<string> paragraphs;

                if (isMarkdown)
                {
                    lines = TextChunker.SplitMarkDownLines(content, maxTokenLength);
                    paragraphs = TextChunker.SplitMarkdownParagraphs(lines, maxTokenLength);
                }
                else
                {
                    lines = TextChunker.SplitPlainTextLines(content, maxTokenLength);
                    paragraphs = TextChunker.SplitPlainTextParagraphs(lines, maxTokenLength);
                }

                for (var i = 0; i < paragraphs.Count; i++)
                {
                    await _kernel.Memory.SaveInformationAsync(
                        AppConstants.KnowledgeBaseCollectionId,
                        text: $"{paragraphs[i]}",
                        id: $"{fileId}_{i}",
                        additionalMetadata: Path.GetFileName(filePath));
                }
            }
            else
            {
                await _kernel.Memory.SaveInformationAsync(
                    AppConstants.KnowledgeBaseCollectionId,
                    text: $"{content}",
                    id: fileId,
                    additionalMetadata: Path.GetFileName(filePath));
            }
        }
        catch (System.Exception ex)
        {
            Logger.Error(ex, $"Error saving context, path: {filePath}");
            _tempMemoryFileFailedCount++;
        }

        _tempMemoryFileImportedCount++;
    }

    private ISKFunction GetQAFunction(SessionOptions options)
    {
        var config = new PromptTemplateConfig
        {
            Description = WorkflowConstants.QADescription,
            Completion =
            {
                MaxTokens = options.MaxResponseTokens,
                FrequencyPenalty = options.FrequencyPenalty,
                PresencePenalty = options.PresencePenalty,
                Temperature = options.Temperature,
                TopP = options.TopP,
            },
        };

        var promptTemplate = new PromptTemplate(WorkflowConstants.QAPrompt, config, _kernel);
        var functionConfig = new SemanticFunctionConfig(config, promptTemplate);
        var function = _kernel.RegisterSemanticFunction(WorkflowConstants.QASkillId, functionConfig);
        return function;
    }

    private void OnVariableChanged(object sender, Type e)
    {
        if (e == typeof(IKernel))
        {
            _kernel = Locator.Current.GetVariable<IKernel>();
            if (_memoryStore != null)
            {
                _kernel.UseMemory(_memoryStore);
            }
        }
    }
}
