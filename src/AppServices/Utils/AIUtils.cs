// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FantasyCopilot.Models.Constants;
using Microsoft.SemanticKernel;
using static FantasyCopilot.AppServices.Utils.BasicUtils;

namespace FantasyCopilot.AppServices.Utils;

internal static class AIUtils
{
    internal static IKernel GetSemanticKernel()
    {
        var currentSource = ReadLocalSetting(SettingNames.AISource, AISource.Azure);
        return currentSource switch
        {
            AISource.Azure => GetAzureOpenAIService(),
            AISource.OpenAI => GetOpenAIService(),
            _ => throw new NotSupportedException()
        };
    }

    private static IKernel GetAzureOpenAIService()
    {
        var key = ReadLocalSetting(SettingNames.AzureOpenAIAccessKey, string.Empty);
        var endPoint = ReadLocalSetting(SettingNames.AzureOpenAIEndpoint, string.Empty);
        var chatModelName = ReadLocalSetting(SettingNames.AzureOpenAIChatModelName, string.Empty);
        var embeddingModelName = ReadLocalSetting(SettingNames.AzureOpenAIEmbeddingModelName, string.Empty);
        var completionModelName = ReadLocalSetting(SettingNames.AzureOpenAICompletionModelName, string.Empty);

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

        return kernelBuilder.Build();
    }

    private static IKernel GetOpenAIService()
    {
        var key = ReadLocalSetting(SettingNames.OpenAIAccessKey, string.Empty);
        var organization = ReadLocalSetting(SettingNames.OpenAIOrganization, string.Empty);
        var chatModelName = ReadLocalSetting(SettingNames.OpenAIChatModelName, string.Empty);
        var embeddingModelName = ReadLocalSetting(SettingNames.OpenAIEmbeddingModelName, string.Empty);
        var completionModelName = ReadLocalSetting(SettingNames.OpenAICompletionModelName, string.Empty);
        var customEndpoint = ReadLocalSetting(SettingNames.OpenAICustomEndpoint, string.Empty);
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

        return kernelBuilder.Build();
    }

    private sealed class ProxyOpenAIHandler : HttpClientHandler
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
