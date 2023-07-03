// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Settings page view model.
/// </summary>
public interface ISettingsPageViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Current package version.
    /// </summary>
    string PackageVersion { get; }

    /// <summary>
    /// App build year.
    /// </summary>
    int BuildYear { get; }

    /// <summary>
    /// Copyright text.
    /// </summary>
    string Copyright { get; }

    /// <summary>
    /// The source of AI.
    /// </summary>
    AISource AiSource { get; set; }

    /// <summary>
    /// The source of translate.
    /// </summary>
    TranslateSource TranslateSource { get; set; }

    /// <summary>
    /// Azure Open AI access key.
    /// </summary>
    string AzureOpenAIAccessKey { get; set; }

    /// <summary>
    /// Azure Open AI chat model name.
    /// </summary>
    string AzureOpenAIChatModelName { get; set; }

    /// <summary>
    /// Azure Open AI embedding model name.
    /// </summary>
    string AzureOpenAIEmbeddingModelName { get; set; }

    /// <summary>
    /// Azure Open AI text completion model name.
    /// </summary>
    string AzureOpenAICompletionModelName { get; set; }

    /// <summary>
    /// Azure Open AI endpoint.
    /// </summary>
    string AzureOpenAIEndpoint { get; set; }

    /// <summary>
    /// Open AI access key.
    /// </summary>
    string OpenAIAccessKey { get; set; }

    /// <summary>
    /// Open AI organization.
    /// </summary>
    string OpenAIOrganization { get; set; }

    /// <summary>
    /// Open AI custom endpoint.
    /// </summary>
    string OpenAICustomEndpoint { get; set; }

    /// <summary>
    /// Open AI chat model name.
    /// </summary>
    string OpenAIChatModelName { get; set; }

    /// <summary>
    /// Open AI embedding model name.
    /// </summary>
    string OpenAIEmbeddingModelName { get; set; }

    /// <summary>
    /// Open AI text completion model name.
    /// </summary>
    string OpenAICompletionModelName { get; set; }

    /// <summary>
    /// Whether to display configuration items for Azure Open AI.
    /// </summary>
    bool IsAzureOpenAIShown { get; }

    /// <summary>
    /// Whether to display configuration items for Open AI.
    /// </summary>
    bool IsOpenAIShown { get; }

    /// <summary>
    /// Whether to display configuration items for Custom AI.
    /// </summary>
    bool IsCustomAIShown { get; }

    /// <summary>
    /// The maximum number of characters a single file can accept for text storage.
    /// </summary>
    int MaxSplitContentLength { get; set; }

    /// <summary>
    /// The maximum number of characters that a single segment can accept when text splitting is triggered.
    /// </summary>
    int MaxParagraphTokenLength { get; set; }

    /// <summary>
    /// The maximum number of contexts returned by the knowledge base.
    /// </summary>
    int ContextLimit { get; set; }

    /// <summary>
    /// Acceptable minimum relevance score.
    /// </summary>
    double MinRelevanceScore { get; set; }

    /// <summary>
    /// Key for the Azure Text-to-Speech service.
    /// </summary>
    string AzureVoiceKey { get; set; }

    /// <summary>
    /// Region of the Azure Text-to-Speech service.
    /// </summary>
    string AzureVoiceRegion { get; set; }

    /// <summary>
    /// Key for the Azure Translate service.
    /// </summary>
    string AzureTranslateKey { get; set; }

    /// <summary>
    /// Region of the Azure Translate service.
    /// </summary>
    string AzureTranslateRegion { get; set; }

    /// <summary>
    /// App ID for the Baidu Translate service.
    /// </summary>
    string BaiduTranslateAppId { get; set; }

    /// <summary>
    /// App key for the Baidu Translate service.
    /// </summary>
    string BaiduTranslateAppKey { get; set; }

    /// <summary>
    /// Whether to display configuration items for Azure Translator.
    /// </summary>
    bool IsAzureTranslateShown { get; }

    /// <summary>
    /// Whether to display configuration items for Baidu Translate service.
    /// </summary>
    bool IsBaiduTranslateShown { get; }

    /// <summary>
    /// The deployment address of Stable diffusion.
    /// </summary>
    string StableDiffusionUrl { get; set; }

    /// <summary>
    /// Whether Chat features are enabled.
    /// </summary>
    bool IsChatEnabled { get; set; }

    /// <summary>
    /// Whether to enable the image feature.
    /// </summary>
    bool IsImageEnabled { get; set; }

    /// <summary>
    /// Whether to enable the voice feature.
    /// </summary>
    bool IsVoiceEnabled { get; set; }

    /// <summary>
    /// Whether to enable the translate feature.
    /// </summary>
    bool IsTranslateEnabled { get; set; }

    /// <summary>
    /// Whether to enable the storage feature.
    /// </summary>
    bool IsStorageEnabled { get; set; }

    /// <summary>
    /// Whether to enable the knowledge base feature.
    /// </summary>
    bool IsKnowledgeEnabled { get; set; }

    /// <summary>
    /// Whether to require an app restart.
    /// </summary>
    bool IsRestartRequest { get; set; }

    /// <summary>
    /// Whether to display the console while the plug-in is running.
    /// </summary>
    bool OpenConsoleWhenPluginRunning { get; set; }

    /// <summary>
    /// Hide application instead of closed after closing the window.
    /// </summary>
    bool HideWhenCloseWindow { get; set; }

    /// <summary>
    /// Render markdown message.
    /// </summary>
    bool MessageUseMarkdown { get; set; }

    /// <summary>
    /// Default conversation type.
    /// </summary>
    ConversationType DefaultConversationType { get; set; }

    /// <summary>
    /// The selected chat connector.
    /// </summary>
    IConnectorConfigViewModel SelectedChatConnector { get; set; }

    /// <summary>
    /// The selected text completion connector.
    /// </summary>
    IConnectorConfigViewModel SelectedTextCompletionConnector { get; set; }

    /// <summary>
    /// The selected embedding connector.
    /// </summary>
    IConnectorConfigViewModel SelectedEmbeddingConnector { get; set; }

    /// <summary>
    /// A collection of connectors to use for chat.
    /// </summary>
    ObservableCollection<IConnectorConfigViewModel> ChatConnectors { get; }

    /// <summary>
    /// A collection of connectors to use for text completion.
    /// </summary>
    ObservableCollection<IConnectorConfigViewModel> TextCompletionConnectors { get; }

    /// <summary>
    /// A collection of connectors to use for embedding.
    /// </summary>
    ObservableCollection<IConnectorConfigViewModel> EmbeddingConnectors { get; }

    /// <summary>
    /// Import app configuration.
    /// </summary>
    IAsyncRelayCommand ImportConfigurationCommand { get; }

    /// <summary>
    /// Export app configuration.
    /// </summary>
    IAsyncRelayCommand ExportConfigurationCommand { get; }

    /// <summary>
    /// Command to open the plug-in folder.
    /// </summary>
    IAsyncRelayCommand OpenPluginFolderCommand { get; }

    /// <summary>
    /// Change the plugin folder.
    /// </summary>
    IAsyncRelayCommand ChangePluginFolderCommand { get; }

    /// <summary>
    /// Open log folder.
    /// </summary>
    IAsyncRelayCommand OpenLogFolderCommand { get; }

    /// <summary>
    /// Restart the app.
    /// </summary>
    IRelayCommand RestartCommand { get; }

    /// <summary>
    /// Initialize settings.
    /// </summary>
    void Initialize();
}
