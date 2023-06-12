// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Models.App.Knowledge;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Knowledge base session view model.
/// </summary>
public sealed partial class KnowledgeBaseSessionViewModel
{
    private readonly IMemoryService _memoryService;
    private readonly IFileToolkit _fileToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IAppViewModel _appViewModel;
    private readonly ILogger<KnowledgeBaseSessionViewModel> _logger;
    private readonly DispatcherQueue _dispatcherQueue;

    private KnowledgeBase _sourceBase;
    private CancellationTokenSource _cancellationTokenSource;
    private string _sourceAdvancedQuery;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private string _quickUserInput;

    [ObservableProperty]
    private string _advancedUserInput;

    [ObservableProperty]
    private ISessionOptionsViewModel _options;

    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private bool _isQuickQA;

    [ObservableProperty]
    private bool _isAdvancedSearch;

    [ObservableProperty]
    private KnowledgeSearchType _searchType;

    [ObservableProperty]
    private bool _isQuickResponding;

    [ObservableProperty]
    private bool _isAdvancedSearchResponding;

    [ObservableProperty]
    private bool _isAdvancedAnswerResponding;

    [ObservableProperty]
    private bool _isChatEmpty;

    [ObservableProperty]
    private bool _isNoContext;

    [ObservableProperty]
    private bool _isNoUserInputWhenAdvancedSearch;

    [ObservableProperty]
    private bool _isInSettings;

    [ObservableProperty]
    private Message _advancedAnswerResult;

    /// <inheritdoc/>
    public event EventHandler RequestScrollToBottom;

    /// <inheritdoc/>
    public ObservableCollection<Message> Messages { get; set; }

    /// <inheritdoc/>
    public ObservableCollection<IKnowledgeContextViewModel> Contexts { get; set; }
}
