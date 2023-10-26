// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Gpt;
using RichasyAssistant.Models.App.Knowledge;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

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
    public SynchronizedObservableCollection<Message> Messages { get; set; }

    /// <inheritdoc/>
    public SynchronizedObservableCollection<IKnowledgeContextViewModel> Contexts { get; set; }
}
