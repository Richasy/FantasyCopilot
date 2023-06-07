// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace FantasyCopilot.Services;

/// <summary>
/// Chat service.
/// </summary>
public sealed partial class KernelService : IKernelService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KernelService"/> class.
    /// </summary>
    public KernelService(
        ISettingsToolkit settingsToolkit,
        ILogger<KernelService> logger)
    {
        _kernel = Locator.Current.GetVariable<IKernel>();
        _logger = logger;
        _settingsToolkit = settingsToolkit;
    }

    /// <inheritdoc/>
    public void ReloadConfig()
    {
        var currentSource = _settingsToolkit.ReadLocalSetting(SettingNames.AISource, AISource.Azure);
        CurrentAISource = currentSource;
        switch (currentSource)
        {
            case AISource.Azure:
                AddAzureOpenAIService();
                break;
            case AISource.OpenAI:
                AddOpenAIService();
                break;
            case AISource.HuggingFace:
                AddHuggingFaceService();
                break;
            default:
                break;
        }

        _logger.LogInformation($"AI source reloaded, current source is {currentSource}");
    }

    private void AddAzureOpenAIService()
    {
        var key = _settingsToolkit.RetrieveSecureString(SettingNames.AzureOpenAIAccessKey);
        var endPoint = _settingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAIEndpoint, string.Empty);
        var chatModelName = _settingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAIChatModelName, string.Empty);
        var embeddingModelName = _settingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAIEmbeddingModelName, string.Empty);
        var completionModelName = _settingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAICompletionModelName, string.Empty);

        var isBaseValid = !string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(endPoint);
        var hasChatModel = !string.IsNullOrEmpty(chatModelName);
        var hasEmbeddingModel = !string.IsNullOrEmpty(embeddingModelName);
        var hasCompletionModel = !string.IsNullOrEmpty(completionModelName);

        var kernelBuilder = new KernelBuilder();
        if (isBaseValid && hasChatModel)
        {
            kernelBuilder.WithAzureChatCompletionService(chatModelName, endPoint, key);
        }

        if (isBaseValid && hasEmbeddingModel)
        {
            kernelBuilder.WithAzureTextEmbeddingGenerationService(embeddingModelName, endPoint, key);
        }

        if (isBaseValid && hasCompletionModel)
        {
            kernelBuilder.WithAzureTextCompletionService(completionModelName, endPoint, key);
        }

        kernelBuilder.WithLogger(_logger);
        SetSupportState(isBaseValid, hasChatModel, hasEmbeddingModel, hasCompletionModel);
        Locator.Current.RegisterVariable(typeof(IKernel), kernelBuilder.Build());
    }

    private void AddOpenAIService()
    {
        var key = _settingsToolkit.RetrieveSecureString(SettingNames.OpenAIAccessKey);
        var organization = _settingsToolkit.ReadLocalSetting(SettingNames.OpenAIOrganization, string.Empty);
        var chatModelName = _settingsToolkit.ReadLocalSetting(SettingNames.OpenAIChatModelName, string.Empty);
        var embeddingModelName = _settingsToolkit.ReadLocalSetting(SettingNames.OpenAIEmbeddingModelName, string.Empty);
        var completionModelName = _settingsToolkit.ReadLocalSetting(SettingNames.OpenAICompletionModelName, string.Empty);
        var isBaseValid = !string.IsNullOrEmpty(key);
        var hasChatModel = !string.IsNullOrEmpty(chatModelName);
        var hasEmbeddingModel = !string.IsNullOrEmpty(embeddingModelName);
        var hasCompletionModel = !string.IsNullOrEmpty(completionModelName);

        var kernelBuilder = new KernelBuilder();
        if (isBaseValid && hasChatModel)
        {
            kernelBuilder.WithOpenAIChatCompletionService(chatModelName, key, organization);
        }

        if (isBaseValid && hasEmbeddingModel)
        {
            kernelBuilder.WithOpenAITextEmbeddingGenerationService(embeddingModelName, key, organization);
        }

        if (isBaseValid && hasCompletionModel)
        {
            kernelBuilder.WithOpenAITextCompletionService(completionModelName, key, organization);
        }

        kernelBuilder.WithLogger(_logger);
        SetSupportState(isBaseValid, hasChatModel, hasEmbeddingModel, hasCompletionModel);
        Locator.Current.RegisterVariable(typeof(IKernel), kernelBuilder.Build());
    }

    private void AddHuggingFaceService()
    {
        var key = _settingsToolkit.RetrieveSecureString(SettingNames.HuggingFaceAccessKey);
        var embeddingModelName = _settingsToolkit.ReadLocalSetting(SettingNames.HuggingFaceEmbeddingModelName, string.Empty);
        var embeddingEndpoint = _settingsToolkit.ReadLocalSetting(SettingNames.HuggingFaceEmbeddingEndpoint, string.Empty);
        var completionModelName = _settingsToolkit.ReadLocalSetting(SettingNames.HuggingFaceCompletionModelName, string.Empty);
        var completionEndpoint = _settingsToolkit.ReadLocalSetting(SettingNames.HuggingFaceCompletionEndpoint, string.Empty);

        var isBaseValid = !string.IsNullOrEmpty(key);
        var hasEmbeddingModel = !string.IsNullOrEmpty(embeddingModelName)
            && !string.IsNullOrEmpty(embeddingEndpoint)
            && Uri.TryCreate(embeddingEndpoint, UriKind.RelativeOrAbsolute, out var _);
        var hasCompletionModel = !string.IsNullOrEmpty(completionModelName)
            && !string.IsNullOrEmpty(completionEndpoint)
            && Uri.TryCreate(completionEndpoint, UriKind.RelativeOrAbsolute, out var _);

        var kernelBuilder = new KernelBuilder();

        if (isBaseValid && hasEmbeddingModel)
        {
            kernelBuilder.WithHuggingFaceTextEmbeddingGenerationService(embeddingModelName, key, embeddingEndpoint);
        }

        if (isBaseValid && hasCompletionModel)
        {
            kernelBuilder.WithHuggingFaceTextCompletionService(completionModelName, key, completionEndpoint);
        }

        kernelBuilder.WithLogger(_logger);
        SetSupportState(isBaseValid, false, hasEmbeddingModel, hasCompletionModel);
        Locator.Current.RegisterVariable(typeof(IKernel), kernelBuilder.Build());
    }

    private void SetSupportState(bool isBaseValid, bool hasChatModel, bool hasEmbeddingModel, bool hasCompletionModel)
    {
        IsChatSupport = isBaseValid && (hasChatModel || hasCompletionModel);
        HasChatModel = hasChatModel;
        IsMemorySupport = IsChatSupport && hasEmbeddingModel;
    }
}
