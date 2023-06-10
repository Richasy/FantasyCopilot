// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.Constants;
using Windows.Storage;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Settings page view model.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    [RelayCommand]
    private async Task ImportConfigurationAsync()
    {
        try
        {
            var authResult = await VerifyUserAsync();
            if (!authResult)
            {
                return;
            }

            var fileObj = await _fileToolkit.PickFileAsync(".json", _appViewModel.MainWindow);
            if (fileObj is not StorageFile file)
            {
                return;
            }

            var json = await FileIO.ReadTextAsync(file).AsTask();
            var appConfig = JsonSerializer.Deserialize<AppConfig>(json);
            if (appConfig == null)
            {
                _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.InvalidConfigFile), InfoType.Error);
                return;
            }

            if (appConfig.AzureOpenAI != null)
            {
                if (!string.IsNullOrEmpty(appConfig.AzureOpenAI.Key))
                {
                    AzureOpenAIAccessKey = appConfig.AzureOpenAI.Key;
                }

                if (!string.IsNullOrEmpty(appConfig.AzureOpenAI.Endpoint))
                {
                    AzureOpenAIEndpoint = appConfig.AzureOpenAI.Endpoint;
                }

                if (!string.IsNullOrEmpty(appConfig.AzureOpenAI.ChatModelName))
                {
                    AzureOpenAIChatModelName = appConfig.AzureOpenAI.ChatModelName;
                }

                if (!string.IsNullOrEmpty(appConfig.AzureOpenAI.EmbeddingModelName))
                {
                    AzureOpenAIEmbeddingModelName = appConfig.AzureOpenAI.EmbeddingModelName;
                }

                if (!string.IsNullOrEmpty(appConfig.AzureOpenAI.CompletionModelName))
                {
                    AzureOpenAICompletionModelName = appConfig.AzureOpenAI.CompletionModelName;
                }
            }

            if (appConfig.OpenAI != null)
            {
                if (!string.IsNullOrEmpty(appConfig.OpenAI.Key))
                {
                    OpenAIAccessKey = appConfig.OpenAI.Key;
                }

                if (!string.IsNullOrEmpty(appConfig.OpenAI.Organization))
                {
                    OpenAIOrganization = appConfig.OpenAI.Organization;
                }

                if (!string.IsNullOrEmpty(appConfig.OpenAI.ChatModelName))
                {
                    OpenAIChatModelName = appConfig.OpenAI.ChatModelName;
                }

                if (!string.IsNullOrEmpty(appConfig.OpenAI.EmbeddingModelName))
                {
                    OpenAIEmbeddingModelName = appConfig.OpenAI.EmbeddingModelName;
                }

                if (!string.IsNullOrEmpty(appConfig.OpenAI.CompletionModelName))
                {
                    OpenAICompletionModelName = appConfig.OpenAI.CompletionModelName;
                }

                if (!string.IsNullOrEmpty(appConfig.OpenAI.Endpoint))
                {
                    OpenAICustomEndpoint = appConfig.OpenAI.Endpoint;
                }
            }

            if (appConfig.AzureVoice != null)
            {
                if (!string.IsNullOrEmpty(appConfig.AzureVoice.Key))
                {
                    AzureVoiceKey = appConfig.AzureVoice.Key;
                }

                if (!string.IsNullOrEmpty(appConfig.AzureVoice.Region))
                {
                    AzureVoiceRegion = appConfig.AzureVoice.Region;
                }
            }

            if (appConfig.AzureTranslate != null)
            {
                if (!string.IsNullOrEmpty(appConfig.AzureTranslate.Key))
                {
                    AzureTranslateKey = appConfig.AzureTranslate.Key;
                }

                if (!string.IsNullOrEmpty(appConfig.AzureTranslate.Region))
                {
                    AzureTranslateRegion = appConfig.AzureTranslate.Region;
                }
            }

            if (appConfig.BaiduTranslate != null)
            {
                if (!string.IsNullOrEmpty(appConfig.BaiduTranslate.Key))
                {
                    BaiduTranslateAppKey = appConfig.BaiduTranslate.Key;
                }

                if (!string.IsNullOrEmpty(appConfig.BaiduTranslate.AppId))
                {
                    BaiduTranslateAppId = appConfig.BaiduTranslate.AppId;
                }
            }

            if (appConfig.StableDiffusion != null)
            {
                if (!string.IsNullOrEmpty(appConfig.StableDiffusion.Url))
                {
                    StableDiffusionUrl = appConfig.StableDiffusion.Url;
                }
            }

            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.ConfigImported), InfoType.Success);
        }
        catch (Exception)
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.ImportConfigFailed), InfoType.Error);
        }
    }

    [RelayCommand]
    private async Task ExportConfigurationAsync()
    {
        try
        {
            var authResult = await VerifyUserAsync();
            if (!authResult)
            {
                return;
            }

            var fileObj = await _fileToolkit.SaveFileAsync("FantasyCopilot_Config.json", _appViewModel.MainWindow);
            if (fileObj is not StorageFile file)
            {
                return;
            }

            var appConfig = new AppConfig
            {
                AzureOpenAI = new AppConfig.AzureOpenAIConfig
                {
                    EmbeddingModelName = AzureOpenAIEmbeddingModelName,
                    CompletionModelName = AzureOpenAICompletionModelName,
                    ChatModelName = AzureOpenAIChatModelName,
                    Endpoint = AzureOpenAIEndpoint,
                    Key = AzureOpenAIAccessKey,
                },
                OpenAI = new AppConfig.OpenAIConfig
                {
                    EmbeddingModelName = OpenAIEmbeddingModelName,
                    CompletionModelName = OpenAICompletionModelName,
                    ChatModelName = OpenAIChatModelName,
                    Organization = OpenAIOrganization,
                    Key = OpenAIAccessKey,
                    Endpoint = OpenAICustomEndpoint,
                },
                AzureVoice = new AppConfig.RegionConfig
                {
                    Key = AzureVoiceKey,
                    Region = AzureVoiceRegion,
                },
                AzureTranslate = new AppConfig.RegionConfig
                {
                    Key = AzureTranslateKey,
                    Region = AzureTranslateRegion,
                },
                BaiduTranslate = new AppConfig.BaiduTranslateConfig
                {
                    Key = BaiduTranslateAppKey,
                    AppId = BaiduTranslateAppId,
                },
                StableDiffusion = new AppConfig.UrlConfig
                {
                    Url = StableDiffusionUrl,
                },
            };

            var json = JsonSerializer.Serialize(appConfig);
            await FileIO.WriteTextAsync(file, json);
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.ConfigExported), InfoType.Success);
        }
        catch (Exception)
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.ExportConfigFailed), InfoType.Error);
        }
    }
}
