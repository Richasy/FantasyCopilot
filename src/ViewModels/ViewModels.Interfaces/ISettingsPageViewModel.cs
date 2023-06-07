// Copyright (c) Fantasy Copilot. All rights reserved.

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
    /// Hugging Face access key.
    /// </summary>
    string HuggingFaceAccessKey { get; set; }

    /// <summary>
    /// Hugging Face embedding model name.
    /// </summary>
    string HuggingFaceEmbeddingModelName { get; set; }

    /// <summary>
    /// Hugging Face embedding model service endpoint.
    /// </summary>
    string HuggingFaceEmbeddingEndpoint { get; set; }

    /// <summary>
    /// Hugging Face text completion model name.
    /// </summary>
    string HuggingFaceCompletionModelName { get; set; }

    /// <summary>
    /// Hugging Face text completion model service endpoint.
    /// </summary>
    string HuggingFaceCompletionEndpoint { get; set; }

    /// <summary>
    /// Whether to display configuration items for Azure Open AI.
    /// </summary>
    bool IsAzureOpenAIShown { get; }

    /// <summary>
    /// Whether to display configuration items for Open AI.
    /// </summary>
    bool IsOpenAIShown { get; }

    /// <summary>
    /// Whether to display configuration items for Hugging Face.
    /// </summary>
    bool IsHuggingFaceShown { get; }

    /// <summary>
    /// The maximum number of characters a single file can accept for text storage.
    /// </summary>
    int MaxSplitContentLength { get; set; }

    /// <summary>
    /// The maximum number of characters that a single segment can accept when text splitting is triggered.
    /// </summary>
    int MaxParagraphTokenLength { get; set; }

    /// <summary>
    /// The maximum number of tokens to respond when the context replies.
    /// </summary>
    int ContextResponseTokenLength { get; set; }

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
    /// Default conversation type.
    /// </summary>
    ConversationType DefaultConversationType { get; set; }

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
