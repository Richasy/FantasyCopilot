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
        var customEndpoint = _settingsToolkit.ReadLocalSetting(SettingNames.OpenAICustomEndpoint, string.Empty);
        var isBaseValid = !string.IsNullOrEmpty(key);
        var hasChatModel = !string.IsNullOrEmpty(chatModelName);
        var hasEmbeddingModel = !string.IsNullOrEmpty(embeddingModelName);
        var hasCompletionModel = !string.IsNullOrEmpty(completionModelName);
        var hasCustomEndpoint = !string.IsNullOrEmpty(customEndpoint) && Uri.TryCreate(customEndpoint, UriKind.Absolute, out var _);

        var kernelBuilder = new KernelBuilder();

        if (hasCustomEndpoint)
        {
            if (isBaseValid && hasChatModel)
            {
                kernelBuilder.WithProxyOpenAIChatCompletionService(chatModelName, key, customEndpoint, organization, setAsDefault: true);
            }

            if (isBaseValid && hasEmbeddingModel)
            {
                kernelBuilder.WithProxyOpenAITextEmbeddingGenerationService(embeddingModelName, key, customEndpoint, organization);
            }

            if (isBaseValid && hasCompletionModel)
            {
                kernelBuilder.WithProxyOpenAITextCompletionService(completionModelName, key, customEndpoint, organization);
            }
        }
        else
        {
            if (isBaseValid && hasChatModel)
            {
                kernelBuilder.WithOpenAIChatCompletionService(chatModelName, key, organization, setAsDefault: true);
            }

            if (isBaseValid && hasEmbeddingModel)
            {
                kernelBuilder.WithOpenAITextEmbeddingGenerationService(embeddingModelName, key, organization);
            }

            if (isBaseValid && hasCompletionModel)
            {
                kernelBuilder.WithOpenAITextCompletionService(completionModelName, key, organization);
            }
        }

        kernelBuilder.WithLogger(_logger);
        SetSupportState(isBaseValid, hasChatModel, hasEmbeddingModel, hasCompletionModel);
        Locator.Current.RegisterVariable(typeof(IKernel), kernelBuilder.Build());
    }

    private void SetSupportState(bool isBaseValid, bool hasChatModel, bool hasEmbeddingModel, bool hasCompletionModel)
    {
        IsChatSupport = isBaseValid && (hasChatModel || hasCompletionModel);
        HasChatModel = hasChatModel;
        IsMemorySupport = IsChatSupport && hasEmbeddingModel;
    }
}
