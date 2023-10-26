// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Settings page view model.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    private void WriteSetting<T>(SettingNames name, T value)
    {
        if (value is string str && string.IsNullOrEmpty(str))
        {
            _settingsToolkit.DeleteLocalSetting(name);
        }
        else
        {
            _settingsToolkit.WriteLocalSetting(name, value);
        }
    }

    partial void OnDefaultConversationTypeChanged(ConversationType value)
        => WriteSetting(SettingNames.LastConversationType, value);

    partial void OnAiSourceChanged(AISource value)
    {
        WriteSetting(SettingNames.AISource, value);
        CheckAISource();
    }

    partial void OnTranslateSourceChanged(TranslateSource value)
    {
        WriteSetting(SettingNames.TranslateSource, value);
        CheckTranslateSource();
        TryClear(Locator.Current.GetService<ITranslatePageViewModel>().SourceLanguages);
        TryClear(Locator.Current.GetService<ITranslatePageViewModel>().TargetLanguages);
    }

    partial void OnAzureOpenAIAccessKeyChanged(string value)
        => WriteSetting(SettingNames.AzureOpenAIAccessKey, value);

    partial void OnAzureOpenAIChatModelNameChanged(string value)
        => WriteSetting(SettingNames.AzureOpenAIChatModelName, value);

    partial void OnAzureOpenAIEmbeddingModelNameChanged(string value)
        => WriteSetting(SettingNames.AzureOpenAIEmbeddingModelName, value);

    partial void OnAzureOpenAICompletionModelNameChanged(string value)
        => WriteSetting(SettingNames.AzureOpenAICompletionModelName, value);

    partial void OnAzureOpenAIEndpointChanged(string value)
        => WriteSetting(SettingNames.AzureOpenAIEndpoint, value);

    partial void OnOpenAIAccessKeyChanged(string value)
        => WriteSetting(SettingNames.OpenAIAccessKey, value);

    partial void OnOpenAICustomEndpointChanging(string value)
    {
        if (!value.StartsWith("http") && !string.IsNullOrEmpty(value))
        {
            value = "https://" + value;
        }

        value = value.TrimEnd('/');
        WriteSetting(SettingNames.OpenAICustomEndpoint, value);
    }

    partial void OnOpenAIChatModelNameChanged(string value)
        => WriteSetting(SettingNames.OpenAIChatModelName, value);

    partial void OnOpenAIEmbeddingModelNameChanged(string value)
        => WriteSetting(SettingNames.OpenAIEmbeddingModelName, value);

    partial void OnOpenAICompletionModelNameChanged(string value)
        => WriteSetting(SettingNames.OpenAICompletionModelName, value);

    partial void OnOpenAIOrganizationChanged(string value)
        => WriteSetting(SettingNames.OpenAIOrganization, value);

    partial void OnAzureVoiceKeyChanged(string value)
        => WriteSetting(SettingNames.AzureVoiceKey, value);

    partial void OnAzureVoiceRegionChanged(string value)
        => WriteSetting(SettingNames.AzureVoiceRegion, value);

    partial void OnAzureTranslateKeyChanged(string value)
        => WriteSetting(SettingNames.AzureTranslateKey, value);

    partial void OnAzureTranslateRegionChanged(string value)
        => WriteSetting(SettingNames.AzureTranslateRegion, value);

    partial void OnBaiduTranslateAppIdChanged(string value)
        => WriteSetting(SettingNames.BaiduTranslateAppId, value);

    partial void OnBaiduTranslateAppKeyChanged(string value)
        => WriteSetting(SettingNames.BaiduTranslateAppKey, value);

    partial void OnStableDiffusionUrlChanged(string value)
    {
        if (!value.StartsWith("http") && !string.IsNullOrEmpty(value))
        {
            value = "http://" + value;
        }

        WriteSetting(SettingNames.StableDiffusionUrl, value.TrimEnd('/'));
    }

    partial void OnIsChatEnabledChanged(bool value)
    {
        WriteSetting(SettingNames.IsChatEnabled, value);
        IsRestartRequest = true;
    }

    partial void OnIsImageEnabledChanged(bool value)
    {
        WriteSetting(SettingNames.IsImageEnabled, value);
        IsRestartRequest = true;
    }

    partial void OnIsVoiceEnabledChanged(bool value)
    {
        WriteSetting(SettingNames.IsVoiceEnabled, value);
        IsRestartRequest = true;
    }

    partial void OnIsTranslateEnabledChanged(bool value)
    {
        WriteSetting(SettingNames.IsTranslateEnabled, value);
        IsRestartRequest = true;
    }

    partial void OnIsStorageEnabledChanged(bool value)
    {
        WriteSetting(SettingNames.IsStorageEnabled, value);
        IsRestartRequest = true;
    }

    partial void OnIsKnowledgeEnabledChanged(bool value)
    {
        WriteSetting(SettingNames.IsKnowledgeEnabled, value);
        IsRestartRequest = true;
    }

    partial void OnMaxSplitContentLengthChanged(int value)
        => WriteSetting(SettingNames.MaxSplitContentLength, value);

    partial void OnMaxParagraphTokenLengthChanged(int value)
        => WriteSetting(SettingNames.MaxParagraphTokenLength, value);

    partial void OnContextLimitChanged(int value)
        => WriteSetting(SettingNames.ContextLimit, value);

    partial void OnMinRelevanceScoreChanged(double value)
        => WriteSetting(SettingNames.ContextMinRelevanceScore, value);

    partial void OnOpenConsoleWhenPluginRunningChanged(bool value)
        => WriteSetting(SettingNames.ShowConsoleWhenPluginRunning, value);

    partial void OnHideWhenCloseWindowChanged(bool value)
        => WriteSetting(SettingNames.HideWhenCloseWindow, value);

    partial void OnMessageUseMarkdownChanged(bool value)
        => WriteSetting(SettingNames.MessageUseMarkdown, value);

    partial void OnSelectedChatConnectorChanged(IConnectorConfigViewModel value)
        => WriteSetting(SettingNames.CustomChatConnectorId, value?.Id ?? string.Empty);

    partial void OnSelectedTextCompletionConnectorChanged(IConnectorConfigViewModel value)
        => WriteSetting(SettingNames.CustomTextCompletionConnectorId, value?.Id ?? string.Empty);

    partial void OnSelectedEmbeddingConnectorChanged(IConnectorConfigViewModel value)
        => WriteSetting(SettingNames.CustomEmbeddingConnectorId, value?.Id ?? string.Empty);
}
