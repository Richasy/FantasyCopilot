﻿// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Application view model.
/// </summary>
public sealed partial class AppViewModel
{
    private readonly IResourceToolkit _resourceToolkit;
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly ICacheToolkit _cacheToolkit;
    private readonly ILogger<AppViewModel> _logger;

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

    [ObservableProperty]
    private bool _isConnectorViewerShown;

    /// <inheritdoc/>
    public event EventHandler BackRequest;

    /// <inheritdoc/>
    public event EventHandler<NavigateEventArgs> NavigateRequest;

    /// <inheritdoc/>
    public event EventHandler<AppTipNotificationEventArgs> RequestShowTip;

    /// <inheritdoc/>
    public event EventHandler<string> RequestShowMessage;

    /// <inheritdoc/>
    public SynchronizedObservableCollection<NavigateItem> NavigateItems { get; }

    /// <inheritdoc/>
    public SynchronizedObservableCollection<IConnectorConfigViewModel> Connectors { get; }

    /// <inheritdoc/>
    public Dictionary<ConnectorType, IConnectorConfigViewModel> ConnectorGroup { get; }

    /// <inheritdoc/>
    public PageType CurrentPage { get; set; }

    /// <inheritdoc/>
    public object MainWindow { get; set; }
}
