// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App.Gpt;
using RichasyAssistant.Models.Constants;
using Windows.Storage;

namespace RichasyAssistant.Toolkits;

/// <summary>
/// Cache toolkit.
/// </summary>
public sealed partial class CacheToolkit
{
    private List<SessionMetadata> _prompts;

    /// <inheritdoc/>
    public event EventHandler PromptListChanged;

    /// <inheritdoc/>
    public async Task InitializeCustomPromptsIfNotReadyAsync()
    {
        if (_prompts != null)
        {
            return;
        }

        var data = await _fileToolkit.GetDataFromFileAsync(AppConstants.FavoritePromptsFileName, new List<SessionMetadata>());
        _prompts = data;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SessionMetadata>> GetCustomPromptsAsync()
    {
        await InitializeCustomPromptsIfNotReadyAsync();
        return _prompts;
    }

    /// <inheritdoc/>
    public async Task AddOrUpdatePromptAsync(SessionMetadata prompt)
    {
        await InitializeCustomPromptsIfNotReadyAsync();
        AddOrUpdatePromptInternal(prompt);
        await SavePromptsInternalAsync();
    }

    /// <inheritdoc/>
    public async Task<bool?> ImportPromptsAsync()
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
            var prompts = JsonSerializer.Deserialize<List<SessionMetadata>>(content);
            if (prompts.Count == 0)
            {
                return false;
            }

            foreach (var prompt in prompts)
            {
                AddOrUpdatePromptInternal(prompt);
            }

            await SavePromptsInternalAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import prompt list");
        }

        return false;
    }

    /// <inheritdoc/>
    public async Task<bool?> ExportPromptsAsync()
    {
        var mainWindow = Locator.Current.GetVariable<Window>();
        var fileObj = await _fileToolkit.SaveFileAsync("Favorite_Prompts.json", mainWindow);
        if (fileObj is not StorageFile file)
        {
            return null;
        }

        var json = JsonSerializer.Serialize(_prompts);
        await FileIO.WriteTextAsync(file, json);
        return true;
    }

    /// <inheritdoc/>
    public async Task DeletePromptAsync(string id)
    {
        var sourceRecord = _prompts.FirstOrDefault(p => p.Id == id);
        if (sourceRecord == null)
        {
            return;
        }

        _prompts.Remove(sourceRecord);
        var json = JsonSerializer.Serialize(_prompts);
        await _fileToolkit.WriteContentAsync(json, AppConstants.FavoritePromptsFileName);
        PromptListChanged?.Invoke(this, EventArgs.Empty);
    }

    private void AddOrUpdatePromptInternal(SessionMetadata prompt)
    {
        if (_prompts.Contains(prompt))
        {
            var index = _prompts.IndexOf(prompt);
            _prompts.Remove(prompt);
            _prompts.Insert(index, prompt);
        }
        else
        {
            _prompts.Add(prompt);
        }
    }

    private async Task SavePromptsInternalAsync()
    {
        var json = JsonSerializer.Serialize(_prompts);
        await _fileToolkit.WriteContentAsync(json, AppConstants.FavoritePromptsFileName);
        PromptListChanged?.Invoke(this, EventArgs.Empty);
    }
}
