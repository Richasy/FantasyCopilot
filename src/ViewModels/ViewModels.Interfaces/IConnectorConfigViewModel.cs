// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App.Connectors;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition for connector config view model.
/// </summary>
public interface IConnectorConfigViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Config id.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Display name.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Whether the chat is supported.
    /// </summary>
    bool SupportChat { get; }

    /// <summary>
    /// Whether chat results are returned as a stream.
    /// </summary>
    bool SupportChatStream { get; }

    /// <summary>
    /// Whether text completion is supported.
    /// </summary>
    bool SupportTextCompletion { get; }

    /// <summary>
    /// Whether text completion results are supported to be returned as streams.
    /// </summary>
    bool SupportTextCompletionStream { get; }

    /// <summary>
    /// Whether embedding is supported.
    /// </summary>
    bool SupportEmbedding { get; }

    /// <summary>
    /// Whether to include a description file.
    /// </summary>
    bool HasReadMe { get; }

    /// <summary>
    /// Whether to include a configuration file.
    /// </summary>
    bool HasConfig { get; }

    /// <summary>
    /// Whether the connector is already launched.
    /// </summary>
    bool IsLaunched { get; }

    /// <summary>
    /// Open the description file.
    /// </summary>
    IRelayCommand OpenReadMeCommand { get; }

    /// <summary>
    /// Open the configuration file.
    /// </summary>
    IRelayCommand OpenConfigCommand { get; }

    /// <summary>
    /// Launch the connector.
    /// </summary>
    IRelayCommand LaunchCommand { get; }

    /// <summary>
    /// Stop connector.
    /// </summary>
    IRelayCommand ExitCommand { get; }

    /// <summary>
    /// Inject config data.
    /// </summary>
    /// <param name="config">Connector config.</param>
    void InjectData(ConnectorConfig config);

    /// <summary>
    /// Get config data.
    /// </summary>
    /// <returns><see cref="ConnectorConfig"/>.</returns>
    ConnectorConfig GetData();
}
