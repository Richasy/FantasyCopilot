// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using RichasyAssistant.Models.App.Knowledge;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Toolkits;

/// <summary>
/// Cache toolkit.
/// </summary>
public sealed partial class CacheToolkit
{
    private List<KnowledgeBase> _knowledgeBases;

    /// <inheritdoc/>
    public event EventHandler KnowledgeBaseListChanged;

    /// <inheritdoc/>
    public async Task InitializeKnowledgeBasesIfNotReadyAsync()
    {
        if (_knowledgeBases != null)
        {
            return;
        }

        var data = await _fileToolkit.GetDataFromFileAsync(AppConstants.KnowledgeBaseFileName, new List<KnowledgeBase>());
        _knowledgeBases = data;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<KnowledgeBase>> GetKnowledgeBasesAsync()
    {
        await InitializeKnowledgeBasesIfNotReadyAsync();
        return _knowledgeBases;
    }

    /// <inheritdoc/>
    public async Task AddOrUpdateKnowledgeBaseAsync(KnowledgeBase config)
    {
        await InitializeCustomPromptsIfNotReadyAsync();
        if (_knowledgeBases.Contains(config))
        {
            var index = _knowledgeBases.IndexOf(config);
            _knowledgeBases.Remove(config);
            _knowledgeBases.Insert(index, config);
        }
        else
        {
            _knowledgeBases.Add(config);
        }

        var json = JsonSerializer.Serialize(_knowledgeBases);
        await _fileToolkit.WriteContentAsync(json, AppConstants.KnowledgeBaseFileName);
        KnowledgeBaseListChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public async Task DeleteKnowledgeBaseAsync(string id)
    {
        var sourceRecord = _knowledgeBases.FirstOrDefault(p => p.Id == id);
        if (sourceRecord == null)
        {
            return;
        }

        _knowledgeBases.Remove(sourceRecord);
        var json = JsonSerializer.Serialize(_knowledgeBases);
        await _fileToolkit.WriteContentAsync(json, AppConstants.KnowledgeBaseFileName);
        KnowledgeBaseListChanged?.Invoke(this, EventArgs.Empty);
    }
}
