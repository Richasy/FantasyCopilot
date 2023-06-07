// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.Models.Constants;

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

        var json = JsonSerializer.Serialize(_semanticSkills);
        await _fileToolkit.WriteContentAsync(json, AppConstants.SemanticSkillsFileName);
        SemanticSkillListChanged?.Invoke(this, EventArgs.Empty);
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
}
