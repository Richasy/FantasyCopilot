// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App.Plugins;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;
using Windows.Storage;

namespace RichasyAssistant.Toolkits;

/// <summary>
/// Cache toolkit.
/// </summary>
public sealed partial class CacheToolkit
{
    private List<PluginConfig> _plugins;

    /// <inheritdoc/>
    public async Task<IEnumerable<PluginConfig>> GetPluginConfigsAsync(bool refresh = false)
    {
        if (_plugins == null || refresh)
        {
            await InitializePluginsAsync();
        }

        return _plugins;
    }

    /// <inheritdoc/>
    public async Task<PluginConfig> GetPluginConfigFromZipAsync(string pluginZipPath)
    {
        var tempFolderPath = ApplicationData.Current.TemporaryFolder.Path;
        var tempConfigPath = Path.Combine(tempFolderPath, WorkflowConstants.PluginConfigFileName);
        await Task.Run(() =>
        {
            using var archive = ZipFile.OpenRead(pluginZipPath);
            archive.Entries
                .FirstOrDefault(p => p.Name == WorkflowConstants.PluginConfigFileName)
                .ExtractToFile(tempConfigPath);
        });

        var fileContent = await File.ReadAllTextAsync(tempConfigPath);
        var config = JsonSerializer.Deserialize<PluginConfig>(fileContent);

        await Task.Run(() => { File.Delete(tempConfigPath); });
        return config;
    }

    /// <inheritdoc/>
    public async Task ImportPluginConfigAsync(PluginConfig config, string pluginZipPath)
    {
        var pluginFolder = Path.Combine(GetPluginFolder(), config.Id);
        if (!Directory.Exists(pluginFolder))
        {
            Directory.CreateDirectory(pluginFolder);
        }

        await Task.Run(() =>
        {
            try
            {
                ZipFile.ExtractToDirectory(pluginZipPath, pluginFolder, true);
            }
            catch (System.Exception)
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

        _plugins?.Add(config);
    }

    /// <inheritdoc/>
    public async Task RemovePluginAsync(string pluginId)
    {
        var folder = GetPluginFolder();
        var pluginFolder = Path.Combine(folder, pluginId);
        if (Directory.Exists(pluginFolder))
        {
            await Task.Run(() =>
            {
                Directory.Delete(pluginFolder, true);
            });
        }

        _plugins?.Remove(_plugins.FirstOrDefault(p => p.Id == pluginId));
    }

    private static string GetPluginFolder()
    {
        var settingsToolkit = Locator.Current.GetService<ISettingsToolkit>();
        var pluginFolder = settingsToolkit.ReadLocalSetting(SettingNames.PluginFolderPath, string.Empty);
        if (string.IsNullOrEmpty(pluginFolder))
        {
            pluginFolder = Path.Combine(ApplicationData.Current.LocalFolder.Path, WorkflowConstants.DefaultPluginFolderName);
        }

        return pluginFolder;
    }

    private static void FormatConfigText(PluginConfig config)
    {
        if (config.Resources == default || config.Resources.Count == 0)
        {
            return;
        }

        var currentCulture = CultureInfo.CurrentUICulture;
        var lanName = currentCulture.Name;
        var resource = FindResourceByLanguage(lanName, config.Resources);
        if (resource == null)
        {
            lanName = currentCulture.TwoLetterISOLanguageName;
            resource = FindResourceByLanguage(lanName, config.Resources);
        }

        resource ??= config.Resources.First().Value;

        config.Name = ReplaceResourceTag(config.Name, resource);
        config.Description = ReplaceResourceTag(config.Description, resource);
        foreach (var command in config.Commands)
        {
            command.Name = ReplaceResourceTag(command.Name, resource);
            command.Description = ReplaceResourceTag(command.Description, resource);

            if (command.Parameters != null && command.Parameters.Count > 0)
            {
                foreach (var item in command.Parameters)
                {
                    item.Description = ReplaceResourceTag(item.Description, resource);
                }
            }

            if (command.ConfigSet != null && command.ConfigSet.Count > 0)
            {
                foreach (var item in command.ConfigSet)
                {
                    item.Title = ReplaceResourceTag(item.Title, resource);

                    if (item.Options != null && item.Options.Count > 0)
                    {
                        foreach (var option in item.Options)
                        {
                            option.DisplayName = ReplaceResourceTag(option.DisplayName, resource);
                        }
                    }
                }
            }
        }
    }

    private static string ReplaceResourceTag(string text, Dictionary<string, string> resources)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        var pattern = @"\{\{\$(.+?)\}\}";
        var matches = Regex.Matches(text, pattern);
        foreach (var match in matches.Cast<Match>())
        {
            var value = match.Groups[1].Value;
            if (resources.ContainsKey(value))
            {
                text = text.Replace($"{{{{${value}}}}}", resources[value]);
            }
        }

        return text;
    }

    private static Dictionary<string, string> FindResourceByLanguage(string language, Dictionary<string, Dictionary<string, string>> resources)
    {
        return resources.Any(p => p.Key.Equals(language, System.StringComparison.OrdinalIgnoreCase))
            ? resources.First(p => p.Key.Equals(language, System.StringComparison.OrdinalIgnoreCase)).Value
            : default;
    }

    private async Task InitializePluginsAsync()
    {
        var pluginFolder = GetPluginFolder();
        if (!Directory.Exists(pluginFolder))
        {
            Directory.CreateDirectory(pluginFolder);
        }

        var configList = Directory.GetFiles(pluginFolder, WorkflowConstants.PluginConfigFileName, new EnumerationOptions { RecurseSubdirectories = true, MaxRecursionDepth = 1 });
        var plugins = new List<PluginConfig>();
        foreach (var configPath in configList)
        {
            var json = await File.ReadAllTextAsync(configPath);
            var config = JsonSerializer.Deserialize<PluginConfig>(json);
            FormatConfigText(config);
            plugins.Add(config);
        }

        _plugins = plugins;
    }
}
