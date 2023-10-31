// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using NLog.Extensions.Logging;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App.Web;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Chat service.
/// </summary>
public sealed partial class KernelService : ServiceBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KernelService"/> class.
    /// </summary>
    public KernelService()
    {
        _kernel = Locator.Current.GetVariable<IKernel>();
    }

    /// <summary>
    /// Reload config.
    /// </summary>
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

        Logger.Info($"AI source reloaded, current source is {currentSource}");
    }

    /// <summary>
    /// Get a list of available models.
    /// </summary>
    /// <param name="source">Model source.</param>
    /// <returns>Model list.</returns>
    public async Task<(IEnumerable<string> ChatModels, IEnumerable<string> TextCompletions, IEnumerable<string> Embeddings)> GetSupportModelsAsync(AISource source)
    {
        using var client = new HttpClient();
        if (source == AISource.Azure)
        {
            var endpoint = _settingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAIEndpoint, string.Empty);
            var key = _settingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAIAccessKey, string.Empty);
            var version = _settingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAIEndpointVersion, "2023-05-15");
            var url = $"{endpoint.TrimEnd('/')}/openai/deployments?api-version={version}";

            var aoaiChatModels = new List<string>();
            var aoaiCompletionModels = new List<string>();
            var aoaiEmbeddingsModels = new List<string>();

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("api-key", key);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<OpenAIDeploymentResponse>(content);
            if (responseData.Data?.Any() ?? false)
            {
                foreach (var item in responseData.Data)
                {
                    var type = JudgeModelType(item.Model);
                    if (string.IsNullOrEmpty(type))
                    {
                        continue;
                    }

                    switch (type)
                    {
                        case "chat":
                            aoaiChatModels.Add(item.Id);
                            break;
                        case "embedding":
                            aoaiEmbeddingsModels.Add(item.Id);
                            break;
                        case "text":
                            aoaiCompletionModels.Add(item.Id);
                            break;
                    }
                }
            }

            return (aoaiChatModels, aoaiCompletionModels, aoaiEmbeddingsModels);
        }
        else if (source == AISource.OpenAI)
        {
            var key = _settingsToolkit.ReadLocalSetting(SettingNames.OpenAIAccessKey, string.Empty);
            var url = $"https://api.openai.com/v1/models";
            var oaiChatModels = new List<string>();
            var oaiCompletionModels = new List<string>();
            var oaiEmbeddingsModels = new List<string>();

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", key);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<OpenAIDeploymentResponse>(content);
            if (responseData.Data?.Any() ?? false)
            {
                foreach (var item in responseData.Data)
                {
                    var type = JudgeModelType(item.Id);
                    if (string.IsNullOrEmpty(type))
                    {
                        continue;
                    }

                    switch (type)
                    {
                        case "chat":
                            oaiChatModels.Add(item.Id);
                            break;
                        case "embedding":
                            oaiEmbeddingsModels.Add(item.Id);
                            break;
                        case "text":
                            oaiCompletionModels.Add(item.Id);
                            break;
                    }
                }
            }

            return (oaiChatModels, oaiCompletionModels, oaiEmbeddingsModels);
        }

        return default;
    }

    private static string JudgeModelType(string modelName)
    {
        if (modelName.Contains("embedding", StringComparison.OrdinalIgnoreCase) || modelName.Contains("search", StringComparison.OrdinalIgnoreCase))
        {
            return "embedding";
        }
        else if (modelName.Contains("gpt"))
        {
            return "chat";
        }
        else if (modelName.Contains("text-"))
        {
            return "text";
        }

        return string.Empty;
    }

    private void AddAzureOpenAIService()
    {
        var key = _settingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAIAccessKey, string.Empty);
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

        kernelBuilder.WithLoggerFactory(LoggerFactory.Create(builder =>
        {
            builder.AddNLog();
        }));
        SetSupportState(isBaseValid, hasChatModel, hasEmbeddingModel, hasCompletionModel);
        Locator.Current.RegisterVariable(typeof(IKernel), kernelBuilder.Build());
    }

    private void AddOpenAIService()
    {
        var key = _settingsToolkit.ReadLocalSetting(SettingNames.OpenAIAccessKey, string.Empty);
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

        kernelBuilder.WithLoggerFactory(LoggerFactory.Create(builder =>
        {
            builder.AddNLog();
        }));
        SetSupportState(isBaseValid, hasChatModel, hasEmbeddingModel, hasCompletionModel);
        Locator.Current.RegisterVariable(typeof(IKernel), kernelBuilder.Build());
    }

    private void AddCustomAIService()
    {
        var kernelBuilder = new KernelBuilder();
        var cacheToolkit = Locator.Current.GetService<ICacheToolkit>();
        var chatConnectorId = _settingsToolkit.ReadLocalSetting(SettingNames.CustomChatConnectorId, string.Empty);
        var textConnectorId = _settingsToolkit.ReadLocalSetting(SettingNames.CustomTextCompletionConnectorId, string.Empty);
        var embeddingConnectorId = _settingsToolkit.ReadLocalSetting(SettingNames.CustomEmbeddingConnectorId, string.Empty);
        var chatConnector = cacheToolkit.GetConnectorFromId(chatConnectorId);
        var textConnector = cacheToolkit.GetConnectorFromId(textConnectorId);
        var embeddingConnector = cacheToolkit.GetConnectorFromId(embeddingConnectorId);

        var hasChatModel = chatConnector != null;
        var hasTextModel = textConnector != null;
        var hasEmbeddingModel = embeddingConnector != null;

        if (hasChatModel)
        {
            kernelBuilder.WithCustomChatCompletionService(chatConnector);
        }

        if (hasTextModel)
        {
            kernelBuilder.WithCustomTextCompletionService(textConnector);
        }

        if (hasEmbeddingModel)
        {
            kernelBuilder.WithCustomTextEmbeddingGenerationService(embeddingConnector);
        }

        SetSupportState(true, hasChatModel, hasEmbeddingModel, hasTextModel);
        Locator.Current.RegisterVariable(typeof(IKernel), kernelBuilder.Build());
        Logger.Info("Custom AI source is not supported yet.");
    }

    private void SetSupportState(bool isBaseValid, bool hasChatModel, bool hasEmbeddingModel, bool hasCompletionModel)
    {
        IsChatSupport = isBaseValid && (hasChatModel || hasCompletionModel);
        HasChatModel = hasChatModel;
        HasTextCompletionModel = hasCompletionModel;
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
