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
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Windows.Storage;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Connector config view model.
/// </summary>
public sealed partial class ConnectorConfigViewModel : ViewModelBase, IConnectorConfigViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectorConfigViewModel"/> class.
    /// </summary>
    public ConnectorConfigViewModel(
        ILogger<ConnectorConfigViewModel> logger,
        IAppViewModel appViewModel,
        IResourceToolkit resourceToolkit)
    {
        _logger = logger;
        _appViewModel = appViewModel;
        _resourceToolkit = resourceToolkit;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _internalId = Guid.NewGuid();
    }

    /// <inheritdoc/>
    public ConnectorConfig GetData()
        => _config;

    /// <inheritdoc/>
    public void InjectData(ConnectorConfig config)
    {
        _config = config;
        Id = config.Id;
        DisplayName = config.Name;
        if (config.Features == null)
        {
            config.Features = new System.Collections.Generic.List<ConnectorFeature>();
        }

        SupportChat = config.Features.Any(f => f.Type == ConnectorConstants.ChatType);
        SupportChatStream = SupportChat && config.Features.First(p => p.Type == ConnectorConstants.ChatType).Endpoints.Any(p => p.Type == ConnectorConstants.ChatStreamType);
        SupportTextCompletion = config.Features.Any(f => f.Type == ConnectorConstants.TextCompletionType);
        SupportTextCompletionStream = SupportTextCompletion && config.Features.First(p => p.Type == ConnectorConstants.TextCompletionType).Endpoints.Any(p => p.Type == ConnectorConstants.TextCompletionStreamType);
        SupportEmbedding = config.Features.Any(f => f.Type == ConnectorConstants.EmbeddingType);
        HasConfig = !string.IsNullOrEmpty(config.ConfigPath);
        HasReadMe = !string.IsNullOrEmpty(config.ReadMe);
        State = ConnectorState.NotStarted;
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

        LogContent = string.Empty;
        var folder = GetConnectorFolder();
        var exePath = Path.Combine(folder, _config.ExecuteName);
        _process = new Process();
        _process.StartInfo.FileName = exePath;
        _process.StartInfo.WorkingDirectory = folder;
        _process.StartInfo.UseShellExecute = false;
        _process.StartInfo.CreateNoWindow = true;
        _process.StartInfo.RedirectStandardOutput = true;
        _process.StartInfo.RedirectStandardError = true;

        _process.OutputDataReceived += OnConnectorOutputDataReceived;
        _process.ErrorDataReceived += OnConnectorErrorDataReceived;

        _process.Exited += OnConnectorExited;
        IsLaunched = true;
        CheckConnectorState();
        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
        _appViewModel.ShowTip(string.Format(_resourceToolkit.GetLocalizedString(StringNames.ConnectorLaunched), DisplayName), InfoType.Information);
    }

    [RelayCommand]
    private void Exit()
    {
        if (_process != null && !_process.HasExited)
        {
            _process.Kill();
            IsLaunched = false;
            State = ConnectorState.NotStarted;
            _appViewModel.ShowTip(string.Format(_resourceToolkit.GetLocalizedString(StringNames.ConnectorClosed), DisplayName), InfoType.Information);
        }

        _process = null;
    }

    private void OnConnectorErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            LogContent += $"{e.Data}\n";
            _logger.LogError($"Connector {_config.Name} throw an error: {e.Data}");

            // Sometimes the error is not an error, but an output.
            // As long as the process does not exit,
            // then we judge that it is connected successfully.
            State = ConnectorState.Connected;
        });
    }

    private void OnConnectorOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        // When we receive the process output, it is determined that the service has started.
        _dispatcherQueue.TryEnqueue(() =>
        {
            LogContent += $"{e.Data}\n";
            State = ConnectorState.Connected;
        });
    }

    private void CheckConnectorState()
    {
        if (!IsLaunched)
        {
            State = ConnectorState.NotStarted;
        }
        else if (IsLaunched)
        {
            if (State == ConnectorState.Connected)
            {
                return;
            }

            State = ConnectorState.Launching;
        }
    }

    private void OnConnectorExited(object sender, EventArgs e)
    {
        _process = null;
        IsLaunched = false;

        _dispatcherQueue.TryEnqueue(() =>
        {
            try
            {
                CheckConnectorState();
                _appViewModel.ShowTip(string.Format(_resourceToolkit.GetLocalizedString(StringNames.ConnectorClosed), DisplayName), InfoType.Information);
            }
            catch (Exception)
            {
            }
        });
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
