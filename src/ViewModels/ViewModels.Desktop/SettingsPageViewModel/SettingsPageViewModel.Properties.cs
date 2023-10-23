// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Authorize;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Settings page view model.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IFileToolkit _fileToolkit;
    private readonly IAppViewModel _appViewModel;
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly ILogger<SettingsPageViewModel> _logger;
    private bool _isAzureModelLoaded;
    private bool _isOpenAIModelLoaded;
    private bool _isModelLoading;

    [ObservableProperty]
    private string _packageVersion;

    [ObservableProperty]
    private int _buildYear;

    [ObservableProperty]
    private string _copyright;

    [ObservableProperty]
    private AISource _aiSource;

    [ObservableProperty]
    private TranslateSource _translateSource;

    [ObservableProperty]
    private string _azureOpenAIAccessKey;

    [ObservableProperty]
    private string _azureOpenAIChatModelName;

    [ObservableProperty]
    private string _azureOpenAIEmbeddingModelName;

    [ObservableProperty]
    private string _azureOpenAICompletionModelName;

    [ObservableProperty]
    private string _azureOpenAIEndpoint;

    [ObservableProperty]
    private string _openAIAccessKey;

    [ObservableProperty]
    private string _openAIOrganization;

    [ObservableProperty]
    private string _openAICustomEndpoint;

    [ObservableProperty]
    private string _openAIChatModelName;

    [ObservableProperty]
    private string _openAIEmbeddingModelName;

    [ObservableProperty]
    private string _openAICompletionModelName;

    [ObservableProperty]
    private string _huggingFaceAccessKey;

    [ObservableProperty]
    private string _huggingFaceEmbeddingModelName;

    [ObservableProperty]
    private string _huggingFaceEmbeddingEndpoint;

    [ObservableProperty]
    private string _huggingFaceCompletionModelName;

    [ObservableProperty]
    private string _huggingFaceCompletionEndpoint;

    [ObservableProperty]
    private bool _isAzureOpenAIShown;

    [ObservableProperty]
    private bool _isOpenAIShown;

    [ObservableProperty]
    private bool _isCustomAIShown;

    [ObservableProperty]
    private int _maxSplitContentLength;

    [ObservableProperty]
    private int _maxParagraphTokenLength;

    [ObservableProperty]
    private int _contextLimit;

    [ObservableProperty]
    private double _minRelevanceScore;

    [ObservableProperty]
    private string _azureVoiceKey;

    [ObservableProperty]
    private string _azureVoiceRegion;

    [ObservableProperty]
    private string _azureTranslateKey;

    [ObservableProperty]
    private string _azureTranslateRegion;

    [ObservableProperty]
    private string _baiduTranslateAppId;

    [ObservableProperty]
    private string _baiduTranslateAppKey;

    [ObservableProperty]
    private bool _isBaiduTranslateShown;

    [ObservableProperty]
    private bool _isAzureTranslateShown;

    [ObservableProperty]
    private string _stableDiffusionUrl;

    [ObservableProperty]
    private bool _isChatEnabled;

    [ObservableProperty]
    private bool _isImageEnabled;

    [ObservableProperty]
    private bool _isVoiceEnabled;

    [ObservableProperty]
    private bool _isTranslateEnabled;

    [ObservableProperty]
    private bool _isStorageEnabled;

    [ObservableProperty]
    private bool _isKnowledgeEnabled;

    [ObservableProperty]
    private bool _isRestartRequest;

    [ObservableProperty]
    private bool _openConsoleWhenPluginRunning;

    [ObservableProperty]
    private bool _hideWhenCloseWindow;

    [ObservableProperty]
    private bool _messageUseMarkdown;

    [ObservableProperty]
    private bool _isConnectorRefreshing;

    [ObservableProperty]
    private bool _isConnectorImporting;

    [ObservableProperty]
    private string _connectorFolderPath;

    [ObservableProperty]
    private int _connectorImportProgress;

    [ObservableProperty]
    private ConversationType _defaultConversationType;

    [ObservableProperty]
    private IConnectorConfigViewModel _selectedChatConnector;

    [ObservableProperty]
    private IConnectorConfigViewModel _selectedTextCompletionConnector;

    [ObservableProperty]
    private IConnectorConfigViewModel _selectedEmbeddingConnector;

    [ObservableProperty]
    private bool _isAuthorizedAppsEmpty;

    /// <inheritdoc/>
    public ObservableCollection<IConnectorConfigViewModel> ChatConnectors { get; }

    /// <inheritdoc/>
    public ObservableCollection<IConnectorConfigViewModel> TextCompletionConnectors { get; }

    /// <inheritdoc/>
    public ObservableCollection<IConnectorConfigViewModel> EmbeddingConnectors { get; }

    /// <inheritdoc/>
    public SynchronizedObservableCollection<string> AzureOpenAIChatModels { get; }

    /// <inheritdoc/>
    public SynchronizedObservableCollection<string> AzureOpenAITextCompletionModels { get; }

    /// <inheritdoc/>
    public SynchronizedObservableCollection<string> AzureOpenAIEmbeddingModels { get; }

    /// <inheritdoc/>
    public SynchronizedObservableCollection<string> OpenAIChatModels { get; }

    /// <inheritdoc/>
    public SynchronizedObservableCollection<string> OpenAITextCompletionModels { get; }

    /// <inheritdoc/>
    public SynchronizedObservableCollection<string> OpenAIEmbeddingModels { get; }

    /// <inheritdoc/>
    public ObservableCollection<AuthorizedApp> AuthorizedApps { get; }
}
