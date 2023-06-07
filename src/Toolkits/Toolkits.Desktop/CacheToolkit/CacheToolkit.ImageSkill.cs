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
    private List<ImageSkillConfig> _imageSkills;

    /// <inheritdoc/>
    public event EventHandler ImageSkillListChanged;

    /// <inheritdoc/>
    public async Task InitializeImageSkillsIfNotReadyAsync()
    {
        if (_imageSkills != null)
        {
            return;
        }

        var data = await _fileToolkit.GetDataFromFileAsync(AppConstants.ImageSkillsFileName, new List<ImageSkillConfig>());
        _imageSkills = data;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ImageSkillConfig>> GetImageSkillsAsync()
    {
        await InitializeImageSkillsIfNotReadyAsync();
        return _imageSkills;
    }

    /// <inheritdoc/>
    public async Task<ImageSkillConfig> GetImageSkillByIdAsync(string skillId)
    {
        await InitializeImageSkillsIfNotReadyAsync();
        return _imageSkills.Any(p => p.Id == skillId)
            ? _imageSkills.First(p => p.Id == skillId)
            : default;
    }

    /// <inheritdoc/>
    public async Task AddOrUpdateImageSkillAsync(ImageSkillConfig config)
    {
        await InitializeImageSkillsIfNotReadyAsync();
        if (_imageSkills.Contains(config))
        {
            var index = _imageSkills.IndexOf(config);
            _imageSkills.Remove(config);
            _imageSkills.Insert(index, config);
        }
        else
        {
            _imageSkills.Add(config);
        }

        var json = JsonSerializer.Serialize(_imageSkills);
        await _fileToolkit.WriteContentAsync(json, AppConstants.ImageSkillsFileName);
        ImageSkillListChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public async Task DeleteImageSkillAsync(string id)
    {
        var sourceRecord = _imageSkills.FirstOrDefault(p => p.Id == id);
        if (sourceRecord == null)
        {
            return;
        }

        _imageSkills.Remove(sourceRecord);
        var json = JsonSerializer.Serialize(_imageSkills);
        await _fileToolkit.WriteContentAsync(json, AppConstants.ImageSkillsFileName);
        ImageSkillListChanged?.Invoke(this, EventArgs.Empty);
    }
}
