﻿// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Connectors;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using Windows.Storage;

namespace FantasyCopilot.Toolkits;

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
            _connectors = caches;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<ConnectorConfig> GetConnectors()
        => _connectors == null
            ? throw new Exception("Connectors not initialized. Call InitializeConnectorsAsync() first.")
            : (IEnumerable<ConnectorConfig>)_connectors;

    /// <inheritdoc/>
    public ConnectorConfig GetConnectorFromId(string connectorId)
        => _connectors.FirstOrDefault(p => p.Id == connectorId);

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
    public async Task ImportConnectorConfigAsync(ConnectorConfig config, string connectorZipPath)
    {
        var connectorFolder = Path.Combine(GetConnectorFolder(), config.Id);
        if (!Directory.Exists(connectorFolder))
        {
            Directory.CreateDirectory(connectorFolder);
        }

        await Task.Run(() =>
        {
            ZipFile.ExtractToDirectory(connectorZipPath, connectorFolder, true);
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
