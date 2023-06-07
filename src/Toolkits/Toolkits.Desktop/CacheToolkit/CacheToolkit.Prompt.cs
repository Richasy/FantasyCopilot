// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.Toolkits;

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

        var json = JsonSerializer.Serialize(_prompts);
        await _fileToolkit.WriteContentAsync(json, AppConstants.FavoritePromptsFileName);
        PromptListChanged?.Invoke(this, EventArgs.Empty);
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
}
