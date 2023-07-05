// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Diagnostics;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App.Connectors;
using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Connector config view model.
/// </summary>
public sealed partial class ConnectorConfigViewModel
{
    private readonly Timer _timer;
    private ConnectorConfig _config;
    private Process _process;
    private bool _isTryConnecting;

    [ObservableProperty]
    private string _id;

    [ObservableProperty]
    private string _displayName;

    [ObservableProperty]
    private bool _supportChat;

    [ObservableProperty]
    private bool _supportChatStream;

    [ObservableProperty]
    private bool _supportTextCompletion;

    [ObservableProperty]
    private bool _supportTextCompletionStream;

    [ObservableProperty]
    private bool _supportEmbedding;

    [ObservableProperty]
    private bool _hasReadMe;

    [ObservableProperty]
    private bool _hasConfig;

    [ObservableProperty]
    private bool _isLaunched;

    [ObservableProperty]
    private ConnectorState _state;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is ConnectorConfigViewModel model && Id == model.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
