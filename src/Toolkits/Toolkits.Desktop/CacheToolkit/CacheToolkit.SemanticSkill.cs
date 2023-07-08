// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.Models.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Windows.Storage;

namespace FantasyCopilot.Toolkits;

/// <summary>
/// Cache toolkit.
/// </summary>
public sealed partial class CacheToolkit
{
    private List<SemanticSkillConfig> _semanticSkills;

    /// <inheritdoc/>
    public event EventHandler SemanticSkillListChanged;

    /// <inheritdoc/>
    public async Task InitializeSemanticSkillsIfNotReadyAsync()
    {
        if (_semanticSkills != null)
        {
            return;
        }

        var data = await _fileToolkit.GetDataFromFileAsync(AppConstants.SemanticSkillsFileName, new List<SemanticSkillConfig>());
        _semanticSkills = data;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SemanticSkillConfig>> GetSemanticSkillsAsync()
    {
        await InitializeSemanticSkillsIfNotReadyAsync();
        return _semanticSkills;
    }

    /// <inheritdoc/>
    public async Task<SemanticSkillConfig> GetSemanticSkillByIdAsync(string skillId)
    {
        await InitializeSemanticSkillsIfNotReadyAsync();
        return _semanticSkills.Any(p => p.Id == skillId)
            ? _semanticSkills.First(p => p.Id == skillId)
            : default;
    }

    /// <inheritdoc/>
    public async Task AddOrUpdateSemanticSkillAsync(SemanticSkillConfig config)
    {
        await InitializeSemanticSkillsIfNotReadyAsync();
        AddOrUpdateSemanticSkillInternal(config);
        await SaveSemanticSkillsInternalAsync();
    }

    /// <inheritdoc/>
    public async Task<bool?> ImportSemanticSkillsAsync()
    {
        var mainWindow = Locator.Current.GetVariable<Window>();
        var fileObj = await _fileToolkit.PickFileAsync(".json", mainWindow);
        if (fileObj is not StorageFile file)
        {
            return null;
        }

        try
        {
            var content = await FileIO.ReadTextAsync(file);
            var configs = JsonSerializer.Deserialize<List<SemanticSkillConfig>>(content);
            if (configs.Count == 0)
            {
                return false;
            }

            foreach (var config in configs)
            {
                AddOrUpdateSemanticSkillInternal(config);
            }

            await SaveSemanticSkillsInternalAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import semantic skill list");
        }

        return false;
    }

    /// <inheritdoc/>
    public async Task<bool?> ExportSemanticSkillsAsync()
    {
        var mainWindow = Locator.Current.GetVariable<Window>();
        var fileObj = await _fileToolkit.SaveFileAsync("Semantic_Skills.json", mainWindow);
        if (fileObj is not StorageFile file)
        {
            return null;
        }

        var json = JsonSerializer.Serialize(_semanticSkills);
        await FileIO.WriteTextAsync(file, json);
        return true;
    }

    /// <inheritdoc/>
    public async Task DeleteSemanticSkillAsync(string id)
    {
        var sourceRecord = _semanticSkills.FirstOrDefault(p => p.Id == id);
        if (sourceRecord == null)
        {
            return;
        }

        _semanticSkills.Remove(sourceRecord);
        var json = JsonSerializer.Serialize(_semanticSkills);
        await _fileToolkit.WriteContentAsync(json, AppConstants.SemanticSkillsFileName);
        SemanticSkillListChanged?.Invoke(this, EventArgs.Empty);
    }

    private void AddOrUpdateSemanticSkillInternal(SemanticSkillConfig config)
    {
        if (_semanticSkills.Contains(config))
        {
            var index = _semanticSkills.IndexOf(config);
            _semanticSkills.Remove(config);
            _semanticSkills.Insert(index, config);
        }
        else
        {
            _semanticSkills.Add(config);
        }
    }

    private async Task SaveSemanticSkillsInternalAsync()
    {
        var json = JsonSerializer.Serialize(_semanticSkills);
        await _fileToolkit.WriteContentAsync(json, AppConstants.SemanticSkillsFileName);
        SemanticSkillListChanged?.Invoke(this, EventArgs.Empty);
    }
}
