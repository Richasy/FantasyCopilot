// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Application view model.
/// </summary>
public sealed partial class AppViewModel
{
    private readonly IResourceToolkit _resourceToolkit;
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly ICacheToolkit _cacheToolkit;
    private readonly ILogger<AppViewModel> _logger;
    private readonly Dictionary<ConnectorType, IConnectorConfigViewModel> _connectorGroup;

    [ObservableProperty]
    private bool _isBackButtonShown;

    [ObservableProperty]
    private bool _isNavigationMenuShown;

    [ObservableProperty]
    private NavigateItem _currentNavigateItem;

    [ObservableProperty]
    private bool _isChatAvailable;

    [ObservableProperty]
    private bool _isVoiceAvailable;

    [ObservableProperty]
    private bool _isImageAvailable;

    [ObservableProperty]
    private bool _isTranslateAvailable;

    [ObservableProperty]
    private bool _isStorageAvailable;

    [ObservableProperty]
    private bool _isKnowledgeAvailable;

    /// <inheritdoc/>
    public event EventHandler BackRequest;

    /// <inheritdoc/>
    public event EventHandler<NavigateEventArgs> NavigateRequest;

    /// <inheritdoc/>
    public event EventHandler<AppTipNotificationEventArgs> RequestShowTip;

    /// <inheritdoc/>
    public event EventHandler<string> RequestShowMessage;

    /// <inheritdoc/>
    public ObservableCollection<NavigateItem> NavigateItems { get; }

    /// <inheritdoc/>
    public ObservableCollection<IConnectorConfigViewModel> Connectors { get; }

    /// <inheritdoc/>
    public PageType CurrentPage { get; set; }

    /// <inheritdoc/>
    public object MainWindow { get; set; }
}
