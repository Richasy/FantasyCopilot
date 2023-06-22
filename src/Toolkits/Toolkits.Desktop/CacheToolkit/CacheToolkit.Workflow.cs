// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        AddOrUpdateWorkflowMetadataInternal(metadata);
        await SaveWorkflowMetadataInternalAsync();
    }

    /// <inheritdoc/>
    public async Task<bool?> ImportWorkflowsAsync()
    {
        var mainWindow = Locator.Current.GetVariable<Window>();
        var zipFileObj = await _fileToolkit.PickFileAsync(".zip", mainWindow);
        if (zipFileObj is not StorageFile zipFile)
        {
            return default;
        }

        try
        {
            var tempFolder = await ApplicationData.Current.TemporaryFolder
                .CreateFolderAsync("importWorkflowTempFolder", CreationCollisionOption.ReplaceExisting)
                .AsTask();

            await Task.Run(async () =>
            {
                ZipFile.ExtractToDirectory(zipFile.Path, tempFolder.Path);
                var metaPath = Path.Combine(tempFolder.Path, "metadata.json");
                if (!File.Exists(metaPath))
                {
                    throw new Exception("Metadata file not found.");
                }

                var metadataJson = await File.ReadAllTextAsync(metaPath);
                var metadatas = JsonSerializer.Deserialize<List<WorkflowMetadata>>(metadataJson);

                File.Delete(metaPath);
                var files = Directory.GetFiles(tempFolder.Path);
                var localWorkflowFolder = Path.Combine(ApplicationData.Current.LocalFolder.Path, AppConstants.LocalWorkflowFolderName);
                if (!Directory.Exists(localWorkflowFolder))
                {
                    Directory.CreateDirectory(localWorkflowFolder);
                }

                foreach (var item in files)
                {
                    File.Move(
                        item,
                        Path.Combine(
                            localWorkflowFolder,
                            Path.GetFileName(item)),
                        true);
                }

                Directory.Delete(tempFolder.Path, true);

                foreach (var item in metadatas)
                {
                    AddOrUpdateWorkflowMetadataInternal(item);
                }

                await SaveWorkflowMetadataInternalAsync();
            });

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import workflows.");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool?> ExportWorkflowsAsync()
    {
        var mainWindow = Locator.Current.GetVariable<Window>();
        var zipFileObj = await _fileToolkit.SaveFileAsync("Saved_Workflows.zip", mainWindow);
        if (zipFileObj is not StorageFile zipFile)
        {
            return default;
        }

        try
        {
            var tempFolder = await ApplicationData.Current.TemporaryFolder
                .CreateFolderAsync("exportWorkflowTempFolder", CreationCollisionOption.ReplaceExisting)
                .AsTask();

            await Task.Run(async () =>
            {
                var zipPath = zipFile.Path;
                await zipFile.DeleteAsync();
                File.Copy(Path.Combine(ApplicationData.Current.LocalFolder.Path, AppConstants.WorkflowFileName), Path.Combine(tempFolder.Path, "metadata.json"), true);
                var localWorkflowFolder = Path.Combine(ApplicationData.Current.LocalFolder.Path, AppConstants.LocalWorkflowFolderName);
                if (!Directory.Exists(localWorkflowFolder))
                {
                    Directory.CreateDirectory(localWorkflowFolder);
                }

                var files = Directory.GetFiles(localWorkflowFolder);
                foreach (var item in files)
                {
                    File.Copy(item, Path.Combine(tempFolder.Path, Path.GetFileName(item)));
                }

                using var zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Create);
                var dict = new DirectoryInfo(tempFolder.Path);
                foreach (var item in dict.GetFiles())
                {
                    zipArchive.CreateEntryFromFile(item.FullName, item.Name);
                }

                Directory.Delete(tempFolder.Path, true);
            });

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export workflows.");
            return false;
        }
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

    private void AddOrUpdateWorkflowMetadataInternal(WorkflowMetadata metadata)
    {
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
    }

    private async Task SaveWorkflowMetadataInternalAsync()
    {
        var json = JsonSerializer.Serialize(_workflows);
        await _fileToolkit.WriteContentAsync(json, AppConstants.WorkflowFileName);
        WorkflowListChanged?.Invoke(this, EventArgs.Empty);
    }
}
