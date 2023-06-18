// Copyright (c) Fantasy Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Settings page view model.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IFileToolkit _fileToolkit;
    private readonly IAppViewModel _appViewModel;

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
    private ConversationType _defaultConversationType;
}
