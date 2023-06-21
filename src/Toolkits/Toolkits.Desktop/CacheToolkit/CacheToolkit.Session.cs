// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Windows.Storage;

namespace FantasyCopilot.Toolkits;

/// <summary>
/// Cache toolkit.
/// </summary>
public sealed partial class CacheToolkit : ICacheToolkit
{
    private readonly IFileToolkit _fileToolkit;
    private readonly ILogger<CacheToolkit> _logger;
    private List<SessionMetadata> _sessions;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheToolkit"/> class.
    /// </summary>
    public CacheToolkit(
        IFileToolkit fileToolkit,
        ILogger<CacheToolkit> logger)
    {
        _fileToolkit = fileToolkit;
        _logger = logger;
    }

    /// <inheritdoc/>
    public event EventHandler SessionListChanged;

    /// <inheritdoc/>
    public async Task InitializeCacheSessionsIfNotReadyAsync()
    {
        if (_sessions != null)
        {
            return;
        }

        var data = await _fileToolkit.GetDataFromFileAsync(AppConstants.SavedSessionFileName, new List<SessionMetadata>());
        _sessions = data;
    }

    /// <inheritdoc/>
    public async Task AddOrUpdateSessionMetadataAsync(SessionMetadata metadata)
    {
        await InitializeCacheSessionsIfNotReadyAsync();
        AddOrUpdateSessionInternal(metadata);
        await SaveSessionMetadataInternalAsync();
    }

    /// <inheritdoc/>
    public async Task<bool?> ImportSessionsAsync()
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
                .CreateFolderAsync("importSessionTempFolder", CreationCollisionOption.ReplaceExisting)
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
                var metadatas = JsonSerializer.Deserialize<List<SessionMetadata>>(metadataJson);

                File.Delete(metaPath);
                var files = Directory.GetFiles(tempFolder.Path);
                foreach (var item in files)
                {
                    File.Move(
                        item,
                        Path.Combine(
                            ApplicationData.Current.LocalFolder.Path,
                            AppConstants.LocalSessionFolderName,
                            Path.GetFileName(item)),
                        true);
                }

                Directory.Delete(tempFolder.Path, true);

                foreach (var item in metadatas)
                {
                    AddOrUpdateSessionInternal(item);
                }

                await SaveSessionMetadataInternalAsync();
            });

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import sessions.");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool?> ExportSessionsAsync()
    {
        var mainWindow = Locator.Current.GetVariable<Window>();
        var zipFileObj = await _fileToolkit.SaveFileAsync("Saved_Sessions.zip", mainWindow);
        if (zipFileObj is not StorageFile zipFile)
        {
            return default;
        }

        try
        {
            var tempFolder = await ApplicationData.Current.TemporaryFolder
                .CreateFolderAsync("exportSessionTempFolder", CreationCollisionOption.ReplaceExisting)
                .AsTask();

            await Task.Run(async () =>
            {
                var zipPath = zipFile.Path;
                await zipFile.DeleteAsync();
                File.Copy(Path.Combine(ApplicationData.Current.LocalFolder.Path, AppConstants.SavedSessionFileName), Path.Combine(tempFolder.Path, "metadata.json"), true);
                var files = Directory.GetFiles(Path.Combine(ApplicationData.Current.LocalFolder.Path, AppConstants.LocalSessionFolderName));
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
            _logger.LogError(ex, "Failed to export sessions.");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteSessionAsync(string id)
    {
        var sourceData = _sessions.FirstOrDefault(p => p.Id == id);
        if (sourceData == null)
        {
            return;
        }

        _sessions.Remove(sourceData);
        var json = JsonSerializer.Serialize(_sessions);
        await _fileToolkit.WriteContentAsync(json, AppConstants.SavedSessionFileName);
        await _fileToolkit.DeleteFileAsync(GetSessionFileName(id));
        SessionListChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public async Task<Session> GetSessionByIdAsync(string sessionId)
    {
        var data = await _fileToolkit.GetDataFromFileAsync<Session>(GetSessionFileName(sessionId), default);
        return data;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SessionMetadata>> GetSessionListAsync()
    {
        await InitializeCacheSessionsIfNotReadyAsync();
        return _sessions;
    }

    /// <inheritdoc/>
    public async Task SaveSessionAsync(Session session)
    {
        session.ModifiedTime = DateTimeOffset.Now;
        var json = JsonSerializer.Serialize(session);
        await _fileToolkit.WriteContentAsync(json, GetSessionFileName(session.Id));
    }

    private static string GetSessionFileName(string id)
        => Path.Combine(AppConstants.LocalSessionFolderName, id + ".json");

    private void AddOrUpdateSessionInternal(SessionMetadata metadata)
    {
        if (_sessions.Contains(metadata))
        {
            var index = _sessions.IndexOf(metadata);
            _sessions.Remove(metadata);
            _sessions.Insert(index, metadata);
        }
        else
        {
            _sessions.Add(metadata);
        }
    }

    private async Task SaveSessionMetadataInternalAsync()
    {
        var json = JsonSerializer.Serialize(_sessions);
        await _fileToolkit.WriteContentAsync(json, AppConstants.SavedSessionFileName);
        SessionListChanged?.Invoke(this, EventArgs.Empty);
    }
}
