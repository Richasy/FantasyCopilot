// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App.Connectors;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;
using Windows.Storage;

namespace RichasyAssistant.Toolkits;

/// <summary>
/// Cache toolkit.
/// </summary>
public sealed partial class CacheToolkit
{
    private List<ConnectorConfig> _connectors;

    /// <inheritdoc/>
    public async Task InitializeConnectorsAsync(bool force = false)
    {
        if (force)
        {
            var connectorFolder = GetConnectorFolder();
            if (!Directory.Exists(connectorFolder))
            {
                Directory.CreateDirectory(connectorFolder);
            }

            var configList = Directory.GetFiles(connectorFolder, ConnectorConstants.ConnectorConfigFileName, new EnumerationOptions { RecurseSubdirectories = true, MaxRecursionDepth = 1 });
            var connectors = new List<ConnectorConfig>();
            foreach (var configPath in configList)
            {
                var json = await File.ReadAllTextAsync(configPath);
                var config = JsonSerializer.Deserialize<ConnectorConfig>(json);
                connectors.Add(config);
            }

            _connectors = connectors;
            await SaveConnectorCacheAsync();
        }
        else
        {
            var caches = await _fileToolkit.GetDataFromFileAsync(ConnectorConstants.ConnectorCacheFileName, new List<ConnectorConfig>());
            _connectors = caches ?? new List<ConnectorConfig>();
        }
    }

    /// <inheritdoc/>
    public IEnumerable<ConnectorConfig> GetConnectors()
        => _connectors == null
            ? throw new Exception("Connectors not initialized. Call InitializeConnectorsAsync() first.")
            : (IEnumerable<ConnectorConfig>)_connectors;

    /// <inheritdoc/>
    public ConnectorConfig GetConnectorFromId(string connectorId)
        => string.IsNullOrEmpty(connectorId) ? default : _connectors.FirstOrDefault(p => p.Id == connectorId);

    /// <inheritdoc/>
    public async Task<ConnectorConfig> GetConnectorConfigFromZipAsync(string connectorZipPath)
    {
        var tempFolderPath = ApplicationData.Current.TemporaryFolder.Path;
        var tempConfigPath = Path.Combine(tempFolderPath, ConnectorConstants.ConnectorConfigFileName);
        await Task.Run(() =>
        {
            using var archive = ZipFile.OpenRead(connectorZipPath);
            archive.Entries
                .FirstOrDefault(p => p.Name == ConnectorConstants.ConnectorConfigFileName)
                .ExtractToFile(tempConfigPath);
        });

        var fileContent = await File.ReadAllTextAsync(tempConfigPath);
        var config = JsonSerializer.Deserialize<ConnectorConfig>(fileContent);

        await Task.Run(() => { File.Delete(tempConfigPath); });
        return config;
    }

    /// <inheritdoc/>
    public async Task ImportConnectorConfigAsync(ConnectorConfig config, string connectorZipPath, Action<int> progressAction)
    {
        var connectorFolder = Path.Combine(GetConnectorFolder(), config.Id);
        if (!Directory.Exists(connectorFolder))
        {
            Directory.CreateDirectory(connectorFolder);
        }

        await Task.Run(() =>
        {
            try
            {
                using var archive = ZipFile.OpenRead(connectorZipPath);
                long totalBytes = 0;
                foreach (var entry in archive.Entries)
                {
                    totalBytes += entry.Length;
                }

                long extractedBytes = 0;
                foreach (var entry in archive.Entries)
                {
                    var fullPath = Path.Combine(connectorFolder, entry.FullName);
                    if (entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\"))
                    {
                        Directory.CreateDirectory(fullPath);
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                        entry.ExtractToFile(fullPath);
                        extractedBytes += entry.Length;
                        var progress = (int)((double)extractedBytes / totalBytes * 100);
                        progressAction(progress);
                    }
                }
            }
            catch (Exception)
            {
                var tempFolderPath = ApplicationData.Current.TemporaryFolder.Path;
                var tempDirectory = new DirectoryInfo(tempFolderPath);
                var files = tempDirectory.GetFiles();
                if (files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        file.Delete();
                    }
                }

                throw;
            }
        });

        _connectors?.Add(config);
        await SaveConnectorCacheAsync();
    }

    /// <inheritdoc/>
    public async Task RemoveConnectorAsync(string connectorId)
    {
        var folder = GetConnectorFolder();
        var pluginFolder = Path.Combine(folder, connectorId);

        if (Directory.Exists(pluginFolder))
        {
            await Task.Run(() =>
            {
                Directory.Delete(pluginFolder, true);
            });
        }

        _plugins?.Remove(_plugins.FirstOrDefault(p => p.Id == connectorId));
        await SaveConnectorCacheAsync();
    }

    private static string GetConnectorFolder()
    {
        var settingsToolkit = Locator.Current.GetService<ISettingsToolkit>();
        var connectorFolder = settingsToolkit.ReadLocalSetting(SettingNames.ConnectorFolderPath, string.Empty);
        if (string.IsNullOrEmpty(connectorFolder))
        {
            connectorFolder = Path.Combine(ApplicationData.Current.LocalFolder.Path, ConnectorConstants.DefaultConnectorFolderName);
        }

        return connectorFolder;
    }

    private async Task SaveConnectorCacheAsync()
    {
        var json = JsonSerializer.Serialize(_connectors);
        await _fileToolkit.WriteContentAsync(json, ConnectorConstants.ConnectorCacheFileName);
    }
}
