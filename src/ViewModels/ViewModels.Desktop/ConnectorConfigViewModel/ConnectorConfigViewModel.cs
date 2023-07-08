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
        config.Features ??= new System.Collections.Generic.List<ConnectorFeature>();
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

        _process = new Process();
        if (!string.IsNullOrEmpty(_config.ExecuteName))
        {
            var exePath = Path.Combine(folder, _config.ExecuteName);
            _process.StartInfo.FileName = exePath;
        }
        else if (!string.IsNullOrEmpty(_config.ScriptFile))
        {
            _process.StartInfo.FileName = "powershell.exe";
            _process.StartInfo.Arguments = $"-ExecutionPolicy Bypass -File \"{Path.Combine(folder, _config.ScriptFile)}\"";
        }
        else if (!string.IsNullOrEmpty(_config.ScriptCommand))
        {
            _process.StartInfo.FileName = "powershell.exe";
            _process.StartInfo.Arguments = $"-ExecutionPolicy Bypass -Command \"{_config.ScriptCommand}\"";
        }
        else
        {
            // no script or execute name, we can't launch the connector.
            IsLaunched = false;
            _logger.LogInformation($"{_config.Name} do not have script or execute file, maybe no need to launch.");
            return;
        }

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
        try
        {
            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            _appViewModel.ShowTip(string.Format(_resourceToolkit.GetLocalizedString(StringNames.ConnectorLaunched), DisplayName), InfoType.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to launch connector {_config.Name}");
            IsLaunched = false;
            State = ConnectorState.NotStarted;
            _process?.Dispose();
            _process = null;
        }
    }

    [RelayCommand]
    private void Exit()
    {
        IsLaunched = false;
        State = ConnectorState.NotStarted;
        if (_process != null && !_process.HasExited)
        {
            _process.Kill();
            _appViewModel.ShowTip(string.Format(_resourceToolkit.GetLocalizedString(StringNames.ConnectorClosed), DisplayName), InfoType.Information);
        }

        CleanPort();
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

    private void CleanPort()
    {
        var port = 0;
        if (Uri.TryCreate(_config.BaseUrl, UriKind.Absolute, out var uri))
        {
            if (uri.Port > 0 && !uri.IsDefaultPort)
            {
                port = uri.Port;
            }
        }

        if (port > 0)
        {
            var command = $"Stop-Process -Id (Get-NetTCPConnection -LocalPort {port}).OwningProcess -Force";
            var psi = new ProcessStartInfo("powershell.exe", command);
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            Process.Start(psi);
            _logger.LogInformation($"The port used by the connector has been released");
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
