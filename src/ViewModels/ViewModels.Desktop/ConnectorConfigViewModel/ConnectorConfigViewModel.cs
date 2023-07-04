// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Connectors;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Windows.Storage;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Connector config view model.
/// </summary>
public sealed partial class ConnectorConfigViewModel : ViewModelBase, IConnectorConfigViewModel
{
    /// <inheritdoc/>
    public ConnectorConfig GetData()
        => _config;

    /// <inheritdoc/>
    public void InjectData(ConnectorConfig config)
    {
        _config = config;
        Id = config.Id;
        DisplayName = config.Name;
        SupportChat = config.Features.Any(f => f.Type == ConnectorConstants.ChatType);
        SupportChatStream = SupportChat && config.Features.First(p => p.Type == ConnectorConstants.ChatType).Endpoints.Any(p => p.Type == ConnectorConstants.ChatStreamType);
        SupportTextCompletion = config.Features.Any(f => f.Type == ConnectorConstants.TextCompletionType);
        SupportTextCompletionStream = SupportTextCompletion && config.Features.First(p => p.Type == ConnectorConstants.TextCompletionType).Endpoints.Any(p => p.Type == ConnectorConstants.TextCompletionStreamType);
        SupportEmbedding = config.Features.Any(f => f.Type == ConnectorConstants.EmbeddingType);
    }

    [RelayCommand]
    private void OpenReadMe()
    {
        if (!HasReadMe)
        {
            return;
        }

        OpenFileInternal(_config.ReadMe);
    }

    [RelayCommand]
    private void OpenConfig()
    {
        if (!HasConfig)
        {
            return;
        }

        OpenFileInternal(_config.ConfigPath);
    }

    [RelayCommand]
    private void Launch()
    {
        if (_process != null && !_process.HasExited)
        {
            return;
        }

        var folder = GetConnectorFolder();
        var exePath = Path.Combine(folder, _config.ExecuteName);
        var process = new Process();
        process.StartInfo.FileName = exePath;
        process.StartInfo.WorkingDirectory = folder;
        process.StartInfo.UseShellExecute = true;

        process.Exited += OnConnectorExited;
        IsLaunched = true;
        process.Start();
    }

    [RelayCommand]
    private void Exit()
    {
        if (_process != null && !_process.HasExited)
        {
            _process.Kill();
        }
    }

    private void OnConnectorExited(object sender, EventArgs e)
    {
        _process = null;
        IsLaunched = false;
    }

    private void OpenFileInternal(string fileName)
    {
        var folder = GetConnectorFolder();
        var readmePath = Path.Combine(folder, fileName);
        var process = new Process();
        process.StartInfo.FileName = readmePath;
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
    }

    private string GetConnectorFolder()
    {
        var settingsToolkit = Locator.Current.GetService<ISettingsToolkit>();
        var connectorFolder = settingsToolkit.ReadLocalSetting(SettingNames.ConnectorFolderPath, string.Empty);
        if (string.IsNullOrEmpty(connectorFolder))
        {
            connectorFolder = Path.Combine(ApplicationData.Current.LocalFolder.Path, ConnectorConstants.DefaultConnectorFolderName);
        }

        return Path.Combine(connectorFolder, Id);
    }
}
