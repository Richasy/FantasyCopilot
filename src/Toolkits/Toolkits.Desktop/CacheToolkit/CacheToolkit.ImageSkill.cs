// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App.Workspace;
using RichasyAssistant.Models.Constants;
using Windows.Storage;

namespace RichasyAssistant.Toolkits;

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
        AddOrUpdateImageSkillInternal(config);
        await SaveImageSkillsInternalAsync();
    }

    /// <inheritdoc/>
    public async Task<bool?> ImportImageSkillsAsync()
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
            var configs = JsonSerializer.Deserialize<List<ImageSkillConfig>>(content);
            if (configs.Count == 0)
            {
                return false;
            }

            foreach (var config in configs)
            {
                AddOrUpdateImageSkillInternal(config);
            }

            await SaveImageSkillsInternalAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import image skill list");
        }

        return false;
    }

    /// <inheritdoc/>
    public async Task<bool?> ExportImageSkillsAsync()
    {
        var mainWindow = Locator.Current.GetVariable<Window>();
        var fileObj = await _fileToolkit.SaveFileAsync("Image_Skills.json", mainWindow);
        if (fileObj is not StorageFile file)
        {
            return null;
        }

        var json = JsonSerializer.Serialize(_imageSkills);
        await FileIO.WriteTextAsync(file, json);
        return true;
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

    private void AddOrUpdateImageSkillInternal(ImageSkillConfig config)
    {
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
    }

    private async Task SaveImageSkillsInternalAsync()
    {
        var json = JsonSerializer.Serialize(_imageSkills);
        await _fileToolkit.WriteContentAsync(json, AppConstants.ImageSkillsFileName);
        ImageSkillListChanged?.Invoke(this, EventArgs.Empty);
    }
}
