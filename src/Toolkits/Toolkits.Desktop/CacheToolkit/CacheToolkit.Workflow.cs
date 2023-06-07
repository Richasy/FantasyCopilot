// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
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
    private List<WorkflowMetadata> _workflows;

    /// <inheritdoc/>
    public event EventHandler WorkflowListChanged;

    /// <inheritdoc/>
    public async Task InitializeWorkflowsIfNotReadyAsync()
    {
        if (_workflows != null)
        {
            return;
        }

        var data = await _fileToolkit.GetDataFromFileAsync(AppConstants.WorkflowFileName, new List<WorkflowMetadata>());
        _workflows = data;
    }

    /// <inheritdoc/>
    public async Task AddOrUpdateWorkflowMetadataAsync(WorkflowMetadata metadata)
    {
        await InitializeWorkflowsIfNotReadyAsync();
        if (_workflows.Contains(metadata))
        {
            var index = _workflows.IndexOf(metadata);
            _workflows.Remove(metadata);
            _workflows.Insert(index, metadata);
        }
        else
        {
            _workflows.Add(metadata);
        }

        var json = JsonSerializer.Serialize(_workflows);
        await _fileToolkit.WriteContentAsync(json, AppConstants.WorkflowFileName);
        WorkflowListChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public async Task DeleteWorkflowAsync(string id)
    {
        var sourceData = _workflows.FirstOrDefault(p => p.Id == id);
        if (sourceData == null)
        {
            return;
        }

        _workflows.Remove(sourceData);
        var json = JsonSerializer.Serialize(_workflows);
        await _fileToolkit.WriteContentAsync(json, AppConstants.WorkflowFileName);
        await _fileToolkit.DeleteFileAsync(GetWorkflowFileName(id));
        WorkflowListChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public async Task<WorkflowConfig> GetWorkflowByIdAsync(string workflowId)
    {
        var data = await _fileToolkit.GetDataFromFileAsync<WorkflowConfig>(GetWorkflowFileName(workflowId), default);
        return data;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<WorkflowMetadata>> GetWorkflowListAsync()
    {
        await InitializeWorkflowsIfNotReadyAsync();
        return _workflows;
    }

    /// <inheritdoc/>
    public async Task SaveWorkflowAsync(WorkflowConfig workflow)
    {
        var json = JsonSerializer.Serialize(workflow);
        await _fileToolkit.WriteContentAsync(json, GetWorkflowFileName(workflow.Id));
    }

    private static string GetWorkflowFileName(string id)
        => Path.Combine(AppConstants.LocalWorkflowFolderName, id + ".json");
}
