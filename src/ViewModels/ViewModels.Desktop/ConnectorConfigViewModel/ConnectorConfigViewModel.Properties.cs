﻿// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App.Connectors;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Connector config view model.
/// </summary>
public sealed partial class ConnectorConfigViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly ILogger<ConnectorConfigViewModel> _logger;
    private readonly IAppViewModel _appViewModel;
    private readonly IResourceToolkit _resourceToolkit;
    private ConnectorConfig _config;
    private Process _process;

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
