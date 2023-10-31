// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Gpt;
using RichasyAssistant.Libs.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Session view model.
/// </summary>
public sealed partial class SessionViewModel
{
    private readonly ICacheToolkit _cacheToolkit;
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly ISessionService _sessionService;
    private readonly IChatService _chatService;
    private readonly DispatcherQueue _dispatcherQueue;

    private Session _sourceSession;
    private SessionMetadata _sourceMetadata;
    private CancellationTokenSource _cancellationTokenSource;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private string _userInput;

    [ObservableProperty]
    private string _tempMessage;

    [ObservableProperty]
    private ISessionOptionsViewModel _options;

    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private bool _isContinuousConversation;

    [ObservableProperty]
    private bool _isSingleConversation;

    [ObservableProperty]
    private Models.Constants.ConversationType _conversationType;

    [ObservableProperty]
    private bool _isResponding;

    [ObservableProperty]
    private bool _isChatEmpty;

    [ObservableProperty]
    private bool _isLocalSession;

    [ObservableProperty]
    private bool _allowCreateNewSession;

    [ObservableProperty]
    private bool _isSessionServiceConnected;

    /// <inheritdoc/>
    public event EventHandler RequestScrollToBottom;

    /// <inheritdoc/>
    public SynchronizedObservableCollection<Message> Messages { get; }

    /// <inheritdoc/>
    public string Id { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is SessionViewModel model && Id == model.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
