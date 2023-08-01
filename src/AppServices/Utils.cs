// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FantasyCopilot.Models.Constants;
using Microsoft.SemanticKernel;
using Windows.Security.Credentials;
using Windows.Storage;

namespace FantasyCopilot.AppServices;

internal static class Utils
{
    internal static T ReadLocalSetting<T>(SettingNames settingName, T defaultValue)
    {
        var settingContainer = ApplicationData.Current.LocalSettings;

        if (IsSettingKeyExist(settingName))
        {
            if (defaultValue is Enum)
            {
                var tempValue = settingContainer.Values[settingName.ToString()].ToString();
                Enum.TryParse(typeof(T), tempValue, out var result);
                return (T)result;
            }
            else
            {
                return (T)settingContainer.Values[settingName.ToString()];
            }
        }
        else
        {
            return defaultValue;
        }
    }

    internal static async Task<IKernel> GetSemanticKernelAsync()
    {
        var currentSource = ReadLocalSetting(SettingNames.AISource, AISource.Azure);
        return currentSource switch
        {
            AISource.Azure => await GetAzureOpenAIServiceAsync(),
            AISource.OpenAI => await GetOpenAIServiceAsync(),
            _ => throw new NotSupportedException()
        };
    }

    private static bool IsSettingKeyExist(SettingNames settingName)
        => ApplicationData.Current.LocalSettings.Values.ContainsKey(settingName.ToString());

    private static async Task<string> RetrieveSecureStringAsync(SettingNames settingName)
    {
        return await Task.Run(() =>
        {
            try
            {
                var name = Assembly.GetAssembly(typeof(Utils)).FullName.Replace("AppServices", "Toolkits");
                var credential = new PasswordVault().Retrieve(name, settingName.ToString());
                credential.RetrievePassword();
                return credential.Password;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        });
    }

    private static async Task<IKernel> GetAzureOpenAIServiceAsync()
    {
        var key = await RetrieveSecureStringAsync(SettingNames.AzureOpenAIAccessKey);
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

    private static async Task<IKernel> GetOpenAIServiceAsync()
    {
        var key = await RetrieveSecureStringAsync(SettingNames.OpenAIAccessKey);
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
