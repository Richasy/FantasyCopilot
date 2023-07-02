// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Connectors;
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
            case AISource.Custom:
                AddCustomAIService();
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
        var customHttpClient = hasCustomEndpoint
            ? new HttpClient(new ProxyOpenAIHandler(customEndpoint))
            : default;

        if (isBaseValid && hasChatModel)
        {
            kernelBuilder.WithOpenAIChatCompletionService(chatModelName, key, organization, setAsDefault: true, httpClient: customHttpClient);
        }

        if (isBaseValid && hasEmbeddingModel)
        {
            kernelBuilder.WithOpenAITextEmbeddingGenerationService(embeddingModelName, key, organization, httpClient: customHttpClient);
        }

        if (isBaseValid && hasCompletionModel)
        {
            kernelBuilder.WithOpenAITextCompletionService(completionModelName, key, organization, httpClient: customHttpClient);
        }

        kernelBuilder.WithLogger(_logger);
        SetSupportState(isBaseValid, hasChatModel, hasEmbeddingModel, hasCompletionModel);
        Locator.Current.RegisterVariable(typeof(IKernel), kernelBuilder.Build());
    }

    private void AddCustomAIService()
    {
        var kernelBuilder = new KernelBuilder();
        var config = new ConnectorConfig
        {
            Id = Guid.NewGuid().ToString(),
            Name = "ChatGLM",
            BaseUrl = "http://127.0.0.1:4399",
            ExecuteName = "api.exe",
            Features = new System.Collections.Generic.List<ConnectorFeature>
            {
                new ConnectorFeature
                {
                    Type = ConnectorConstants.ChatType,
                    Endpoints = new System.Collections.Generic.List<ConnectorEndpoint>
                    {
                        new ConnectorEndpoint
                        {
                            Type = ConnectorConstants.ChatRestType,
                            Path = "/chat",
                        },
                    },
                },
            },
        };
        kernelBuilder.WithCustomChatCompletionService(config);
        SetSupportState(true, true, false, false);
        Locator.Current.RegisterVariable(typeof(IKernel), kernelBuilder.Build());
        _logger.LogInformation("Custom AI source is not supported yet.");
    }

    private void SetSupportState(bool isBaseValid, bool hasChatModel, bool hasEmbeddingModel, bool hasCompletionModel)
    {
        IsChatSupport = isBaseValid && (hasChatModel || hasCompletionModel);
        HasChatModel = hasChatModel;
        IsMemorySupport = IsChatSupport && hasEmbeddingModel;
    }

    private class ProxyOpenAIHandler : HttpClientHandler
    {
        private readonly string _proxyUrl;

        public ProxyOpenAIHandler(string proxyUrl)
            => _proxyUrl = proxyUrl.TrimEnd('/');

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri != null && request.RequestUri.Host.Equals("api.openai.com", StringComparison.OrdinalIgnoreCase))
            {
                var path = request.RequestUri.PathAndQuery.TrimStart('/');
                request.RequestUri = new Uri($"{_proxyUrl}/{path}");
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
