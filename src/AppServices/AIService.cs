// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.AI.Translation.Text;
using FantasyCopilot.AppServices.Utils;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Authorize;
using FantasyCopilot.Models.Constants;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using static FantasyCopilot.Models.App.AppConfig;

namespace FantasyCopilot.AppServices;

/// <summary>
/// Quick chat service.
/// </summary>
public sealed class AIService : IBackgroundTask
{
    private BackgroundTaskDeferral _deferral;
    private AppServiceConnection _connection;
    private CancellationTokenSource _cancellation;

    /// <inheritdoc/>
    public void Run(IBackgroundTaskInstance taskInstance)
    {
        _deferral = taskInstance.GetDeferral();

        taskInstance.Canceled += OnTaskInstanceCanceled;

        var detail = taskInstance.TriggerDetails as AppServiceTriggerDetails;
        _connection = detail.AppServiceConnection;
        _connection.RequestReceived += OnConnectionRequestReceivedAsync;
    }

    private static async Task<bool> CheckConfigRequestAsync(AppServiceConnection connection, ValueSet returnData, string packageId, string scope)
    {
        if (string.IsNullOrEmpty(packageId))
        {
            returnData.Add("Error", "Package ID is not available");
            return false;
        }
        else
        {
            var appList = await BasicUtils.GetDataFromFileAsync(AppConstants.AuthorizedAppsFileName, new List<AuthorizedApp>());
            var isAuthorized = appList.Any(p => p.PackageId.Equals(packageId) && p.Scopes.Any(j => j == scope));
            if (!isAuthorized)
            {
                returnData.Add("Error", "App is not authorized");
                return false;
            }
        }

        return true;
    }

    private async void OnConnectionRequestReceivedAsync(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
    {
        var msgDeferral = args.GetDeferral();

        var msg = args.Request.Message;
        var returnData = new ValueSet();

        var command = msg["Command"] as string;
        var request = msg["Request"] as string;

        if (!string.IsNullOrEmpty(command))
        {
            _cancellation = new CancellationTokenSource();
            try
            {
                if (command.Equals("quickchat", StringComparison.InvariantCultureIgnoreCase))
                {
                    var data = JsonSerializer.Deserialize<QuickChatPrompt>(request);
                    var kernel = AIUtils.GetSemanticKernel();

                    if (kernel == null)
                    {
                        returnData.Add("Error", "Kernel is not available");
                    }

                    if (data.UseChat)
                    {
                        var chatCompletion = kernel.GetService<IChatCompletion>() ?? throw new Exception("Chat completion service is not available");
                        var requestSettings = new ChatRequestSettings
                        {
                            MaxTokens = data.MaxResponseTokens == 0 ? 600 : data.MaxResponseTokens,
                            Temperature = data.Temperature,
                            FrequencyPenalty = 1,
                            PresencePenalty = 1,
                            TopP = 0,
                        };
                        var chatHistory = chatCompletion.CreateNewChat();
                        chatHistory.AddUserMessage(data.ActualMessage);
                        var response = await chatCompletion.GenerateMessageAsync(chatHistory, requestSettings, _cancellation.Token);
                        returnData.Add("Response", response);
                    }
                    else
                    {
                        var textCompletion = kernel.GetService<ITextCompletion>() ?? throw new Exception("Text completion service is not available");
                        var requestSettings = new CompleteRequestSettings
                        {
                            MaxTokens = data.MaxResponseTokens == 0 ? 600 : data.MaxResponseTokens,
                            Temperature = data.Temperature,
                            FrequencyPenalty = 1,
                            PresencePenalty = 1,
                            TopP = 0,
                        };

                        var response = await textCompletion.CompleteAsync(data.ActualMessage, requestSettings, _cancellation.Token);
                        returnData.Add("Response", response);
                    }
                }
                else if (command.Equals("checkaccess", StringComparison.InvariantCultureIgnoreCase))
                {
                    var scopes = msg["Scopes"] as string;
                    var scopeList = scopes.Split(',');
                    var packageId = sender.PackageFamilyName;
                    if (string.IsNullOrEmpty(packageId))
                    {
                        returnData.Add("Error", "Package ID is not available");
                    }
                    else
                    {
                        var appList = await BasicUtils.GetDataFromFileAsync(AppConstants.AuthorizedAppsFileName, new List<AuthorizedApp>());
                        var app = appList.FirstOrDefault(p => p.PackageId.Equals(packageId));
                        if (app == null || scopeList.Except(app.Scopes).Any())
                        {
                            returnData.Add("Error", "App not registered or scope changed.");
                        }
                        else
                        {
                            returnData.Add("Response", "OK");
                        }
                    }
                }
                else if (command.Equals("getaiconfig", StringComparison.InvariantCultureIgnoreCase))
                {
                    var isAuthorized = await CheckConfigRequestAsync(sender, returnData, request, "oai");
                    if (isAuthorized)
                    {
                        // Get the configuration of Azure Open AI or Open AI.
                        var config = new AiConfigResponse();
                        var openAiKey = BasicUtils.ReadLocalSetting(SettingNames.OpenAIAccessKey, string.Empty);
                        if (!string.IsNullOrEmpty(openAiKey))
                        {
                            var oaiConfig = new OpenAIConfig();
                            oaiConfig.Key = openAiKey;
                            oaiConfig.Organization = BasicUtils.ReadLocalSetting(SettingNames.OpenAIOrganization, string.Empty);
                            oaiConfig.Endpoint = BasicUtils.ReadLocalSetting(SettingNames.OpenAICustomEndpoint, string.Empty);
                            oaiConfig.ChatModelName = BasicUtils.ReadLocalSetting(SettingNames.OpenAIChatModelName, string.Empty);
                            oaiConfig.CompletionModelName = BasicUtils.ReadLocalSetting(SettingNames.OpenAICompletionModelName, string.Empty);
                            oaiConfig.EmbeddingModelName = BasicUtils.ReadLocalSetting(SettingNames.OpenAIEmbeddingModelName, string.Empty);
                            config.OpenAI = oaiConfig;
                        }

                        var azOpenAiKey = BasicUtils.ReadLocalSetting(SettingNames.AzureOpenAIAccessKey, string.Empty);
                        if (!string.IsNullOrEmpty(azOpenAiKey))
                        {
                            var azOaiConfig = new AzureOpenAIConfig();
                            azOaiConfig.Key = azOpenAiKey;
                            azOaiConfig.Endpoint = BasicUtils.ReadLocalSetting(SettingNames.AzureOpenAIEndpoint, string.Empty);
                            azOaiConfig.ChatModelName = BasicUtils.ReadLocalSetting(SettingNames.AzureOpenAIChatModelName, string.Empty);
                            azOaiConfig.CompletionModelName = BasicUtils.ReadLocalSetting(SettingNames.AzureOpenAICompletionModelName, string.Empty);
                            azOaiConfig.EmbeddingModelName = BasicUtils.ReadLocalSetting(SettingNames.AzureOpenAIEmbeddingModelName, string.Empty);
                            config.AzureOpenAI = azOaiConfig;
                        }

                        var preferredAI = BasicUtils.ReadLocalSetting(SettingNames.AISource, AISource.Azure);
                        var preferredStr = preferredAI == AISource.Azure ? "azoai" : "oai";
                        if (preferredStr == "oai" && string.IsNullOrEmpty(openAiKey))
                        {
                            preferredStr = "azoai";
                        }

                        config.PreferredAI = preferredStr;
                        returnData.Add("Response", JsonSerializer.Serialize(config));
                    }
                }
                else if (command.Equals("gettranslateconfig", StringComparison.InvariantCultureIgnoreCase))
                {
                    var isAuthorized = await CheckConfigRequestAsync(sender, returnData, request, "translate");
                    if (isAuthorized)
                    {
                        var config = new TranslateConfigResponse();
                        var azTranslateKey = BasicUtils.ReadLocalSetting(SettingNames.AzureTranslateKey, string.Empty);
                        if (!string.IsNullOrEmpty(azTranslateKey))
                        {
                            var azTranslateConfig = new RegionConfig
                            {
                                Key = azTranslateKey,
                                Region = BasicUtils.ReadLocalSetting(SettingNames.AzureTranslateRegion, string.Empty),
                            };
                            config.AzureTranslate = azTranslateConfig;
                        }

                        var baiduTranslateKey = BasicUtils.ReadLocalSetting(SettingNames.BaiduTranslateAppKey, string.Empty);
                        if (!string.IsNullOrEmpty(baiduTranslateKey))
                        {
                            var baiduTranslateConfig = new BaiduTranslateConfig
                            {
                                Key = baiduTranslateKey,
                                AppId = BasicUtils.ReadLocalSetting(SettingNames.BaiduTranslateAppId, string.Empty),
                            };
                            config.BaiduTranslate = baiduTranslateConfig;
                        }

                        var preferredTranslate = BasicUtils.ReadLocalSetting(SettingNames.TranslateSource, TranslateSource.Azure);
                        var preferredStr = preferredTranslate == TranslateSource.Azure ? "azure" : "baidu";
                        returnData.Add("Response", JsonSerializer.Serialize(config));
                    }
                }
                else if (command.Equals("getvoiceconfig", StringComparison.InvariantCultureIgnoreCase))
                {
                    var isAuthorized = await CheckConfigRequestAsync(sender, returnData, request, "voice");
                    if (isAuthorized)
                    {
                        var config = new VoiceConfigResponse();
                        var azVoiceKey = BasicUtils.ReadLocalSetting(SettingNames.AzureVoiceKey, string.Empty);
                        if (!string.IsNullOrEmpty(azVoiceKey))
                        {
                            var azVoiceConfig = new RegionConfig
                            {
                                Key = azVoiceKey,
                                Region = BasicUtils.ReadLocalSetting(SettingNames.AzureVoiceRegion, string.Empty),
                            };
                            config.AzureVoice = azVoiceConfig;
                        }

                        returnData.Add("Response", JsonSerializer.Serialize(config));
                    }
                }
                else if (command.Equals("translate", StringComparison.InvariantCultureIgnoreCase))
                {
                    var data = JsonSerializer.Deserialize<QuickTranslateRequest>(request);
                    var translateServiceType = BasicUtils.ReadLocalSetting(SettingNames.TranslateSource, TranslateSource.Azure);
                    if (translateServiceType == TranslateSource.Azure)
                    {
                        var textType = data.Type == "html" ? TextType.Html : TextType.Plain;
                        var translateService = new AzureTranslateUtils();
                        var translateContent = await translateService.TranslateTextAsync(data.Content, data.TargetLanguage, textType, _cancellation.Token);
                        var result = new
                        {
                            Content = translateContent,
                            data.Index,
                        };
                        returnData.Add("Response", JsonSerializer.Serialize(result));
                    }
                }
            }
            catch (Exception ex)
            {
                returnData.Add("Error", ex.Message);
            }

            _cancellation = null;
        }
        else
        {
            returnData.Add("Error", "Invalid command");
        }

        try
        {
            await args.Request.SendResponseAsync(returnData);
        }
        catch (Exception)
        {
        }
        finally
        {
            msgDeferral.Complete();
        }
    }

    private void OnTaskInstanceCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
    {
        _cancellation?.Cancel();
        _deferral?.Complete();
    }
}
