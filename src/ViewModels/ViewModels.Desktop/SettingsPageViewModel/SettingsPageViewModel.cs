// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Windows.ApplicationModel;
using Windows.Security.Credentials.UI;
using Windows.Storage;
using Windows.System;
using WinRT.Interop;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Settings page view model.
/// </summary>
public sealed partial class SettingsPageViewModel : ViewModelBase, ISettingsPageViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPageViewModel"/> class.
    /// </summary>
    public SettingsPageViewModel(
        ISettingsToolkit settingsToolkit,
        IResourceToolkit resourceToolkit,
        IFileToolkit fileToolkit,
        IAppViewModel appViewModel)
    {
        _settingsToolkit = settingsToolkit;
        _resourceToolkit = resourceToolkit;
        _fileToolkit = fileToolkit;
        _appViewModel = appViewModel;
        BuildYear = 2023;
        Initialize();
    }

    /// <inheritdoc/>
    public void Initialize()
    {
        DefaultConversationType = _settingsToolkit.ReadLocalSetting(SettingNames.LastConversationType, ConversationType.Continuous);
        var copyrightTemplate = _resourceToolkit.GetLocalizedString(StringNames.Copyright);
        Copyright = string.Format(copyrightTemplate, BuildYear);
        var version = Package.Current.Id.Version;
        PackageVersion = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        AiSource = _settingsToolkit.ReadLocalSetting(SettingNames.AISource, AISource.Azure);

        AzureOpenAIAccessKey = _settingsToolkit.RetrieveSecureString(SettingNames.AzureOpenAIAccessKey);
        AzureOpenAIChatModelName = _settingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAIChatModelName, string.Empty);
        AzureOpenAICompletionModelName = _settingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAICompletionModelName, string.Empty);
        AzureOpenAIEmbeddingModelName = _settingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAIEmbeddingModelName, string.Empty);
        AzureOpenAIEndpoint = _settingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAIEndpoint, string.Empty);

        OpenAIAccessKey = _settingsToolkit.RetrieveSecureString(SettingNames.OpenAIAccessKey);
        OpenAIOrganization = _settingsToolkit.ReadLocalSetting(SettingNames.OpenAIOrganization, string.Empty);
        OpenAIChatModelName = _settingsToolkit.ReadLocalSetting(SettingNames.OpenAIChatModelName, string.Empty);
        OpenAICompletionModelName = _settingsToolkit.ReadLocalSetting(SettingNames.OpenAICompletionModelName, string.Empty);
        OpenAIEmbeddingModelName = _settingsToolkit.ReadLocalSetting(SettingNames.OpenAIEmbeddingModelName, string.Empty);

        HuggingFaceAccessKey = _settingsToolkit.RetrieveSecureString(SettingNames.HuggingFaceAccessKey);
        HuggingFaceCompletionModelName = _settingsToolkit.ReadLocalSetting(SettingNames.HuggingFaceCompletionModelName, string.Empty);
        HuggingFaceEmbeddingModelName = _settingsToolkit.ReadLocalSetting(SettingNames.HuggingFaceEmbeddingModelName, string.Empty);
        HuggingFaceCompletionEndpoint = _settingsToolkit.ReadLocalSetting(SettingNames.HuggingFaceCompletionEndpoint, string.Empty);
        HuggingFaceEmbeddingEndpoint = _settingsToolkit.ReadLocalSetting(SettingNames.HuggingFaceEmbeddingEndpoint, string.Empty);

        MaxSplitContentLength = _settingsToolkit.ReadLocalSetting(SettingNames.MaxSplitContentLength, 1024);
        MaxParagraphTokenLength = _settingsToolkit.ReadLocalSetting(SettingNames.MaxParagraphTokenLength, 512);
        ContextResponseTokenLength = _settingsToolkit.ReadLocalSetting(SettingNames.ContextResponseTokenLength, 512);

        AzureVoiceKey = _settingsToolkit.RetrieveSecureString(SettingNames.AzureVoiceKey);
        AzureVoiceRegion = _settingsToolkit.ReadLocalSetting(SettingNames.AzureVoiceRegion, string.Empty);

        AzureTranslateKey = _settingsToolkit.RetrieveSecureString(SettingNames.AzureTranslateKey);
        AzureTranslateRegion = _settingsToolkit.ReadLocalSetting(SettingNames.AzureTranslateRegion, string.Empty);

        StableDiffusionUrl = _settingsToolkit.ReadLocalSetting(SettingNames.StableDiffusionUrl, string.Empty);

        OpenConsoleWhenPluginRunning = _settingsToolkit.ReadLocalSetting(SettingNames.ShowConsoleWhenPluginRunning, false);

        IsChatEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsChatEnabled, true);
        IsImageEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsImageEnabled, true);
        IsVoiceEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsVoiceEnabled, true);
        IsTranslateEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsTranslateEnabled, true);
        IsStorageEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsStorageEnabled, true);
        IsKnowledgeEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsKnowledgeEnabled, true);
        IsRestartRequest = false;

        CheckAISource();
    }

    [RelayCommand]
    private static void Restart()
        => Microsoft.Windows.AppLifecycle.AppInstance.Restart(string.Empty);

    [RelayCommand]
    private static async Task OpenLogFolderAsync()
    {
        var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(AppConstants.LogFolderName);
        if (folder != null)
        {
            await Launcher.LaunchFolderAsync(folder);
        }
    }

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
            }

            if (appConfig.HuggingFace != null)
            {
                if (!string.IsNullOrEmpty(appConfig.HuggingFace.Key))
                {
                    HuggingFaceAccessKey = appConfig.HuggingFace.Key;
                }

                if (!string.IsNullOrEmpty(appConfig.HuggingFace.EmbeddingEndpoint))
                {
                    HuggingFaceEmbeddingEndpoint = appConfig.HuggingFace.EmbeddingEndpoint;
                }

                if (!string.IsNullOrEmpty(appConfig.HuggingFace.EmbeddingModelName))
                {
                    HuggingFaceEmbeddingModelName = appConfig.HuggingFace.EmbeddingModelName;
                }

                if (!string.IsNullOrEmpty(appConfig.HuggingFace.CompletionModelName))
                {
                    HuggingFaceCompletionModelName = appConfig.HuggingFace.CompletionModelName;
                }

                if (!string.IsNullOrEmpty(appConfig.HuggingFace.CompletionEndpoint))
                {
                    HuggingFaceCompletionEndpoint = appConfig.HuggingFace.CompletionEndpoint;
                }
            }

            if (appConfig.Voice != null)
            {
                if (!string.IsNullOrEmpty(appConfig.Voice.Key))
                {
                    AzureVoiceKey = appConfig.Voice.Key;
                }

                if (!string.IsNullOrEmpty(appConfig.Voice.Region))
                {
                    AzureVoiceRegion = appConfig.Voice.Region;
                }
            }

            if (appConfig.Translate != null)
            {
                if (!string.IsNullOrEmpty(appConfig.Translate.Key))
                {
                    AzureTranslateKey = appConfig.Translate.Key;
                }

                if (!string.IsNullOrEmpty(appConfig.Translate.Region))
                {
                    AzureTranslateRegion = appConfig.Translate.Region;
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
                },
                HuggingFace = new AppConfig.HuggingFaceConfig
                {
                    EmbeddingModelName = HuggingFaceEmbeddingModelName,
                    CompletionModelName = HuggingFaceCompletionModelName,
                    EmbeddingEndpoint = HuggingFaceEmbeddingEndpoint,
                    CompletionEndpoint = HuggingFaceCompletionEndpoint,
                    Key = HuggingFaceAccessKey,
                },
                Voice = new AppConfig.RegionConfig
                {
                    Key = AzureVoiceKey,
                    Region = AzureVoiceRegion,
                },
                Translate = new AppConfig.RegionConfig
                {
                    Key = AzureTranslateKey,
                    Region = AzureTranslateRegion,
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

    [RelayCommand]
    private async Task OpenPluginFolderAsync()
    {
        var folderPath = GetPluginFolder();
        var folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
        await Launcher.LaunchFolderAsync(folder);
    }

    [RelayCommand]
    private async Task ChangePluginFolderAsync()
    {
        var folderObj = await _fileToolkit.PickFolderAsync(_appViewModel.MainWindow);
        if (folderObj is not StorageFolder folder)
        {
            return;
        }

        var sourceFolder = GetPluginFolder();
        if (sourceFolder == folder.Path)
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.CanNotSelectSameFolder), InfoType.Error);
            return;
        }

        if (!Directory.GetFiles(folder.Path).Any())
        {
            var plugins = Directory.GetDirectories(sourceFolder);
            var tasks = new List<Task>();
            foreach (var pluginFolder in plugins)
            {
                tasks.Add(Task.Run(() =>
                {
                    var plugFolder = pluginFolder;
                    var pluginName = new DirectoryInfo(plugFolder).Name;
                    var destPath = Path.Combine(folder.Path, pluginName);
                    if (Directory.Exists(destPath))
                    {
                        Directory.Delete(destPath, true);
                    }

                    Directory.Move(plugFolder, destPath);
                }));
            }

            await Task.WhenAll(tasks);
        }

        _settingsToolkit.WriteLocalSetting(SettingNames.PluginFolderPath, folder.Path);
        _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.MoveCompleted), InfoType.Success);
        var pluginVM = Locator.Current.GetService<IPluginsModuleViewModel>();
        pluginVM.ReloadPluginCommand.Execute(default);
    }

    private void CheckAISource()
    {
        IsAzureOpenAIShown = AiSource == AISource.Azure;
        IsOpenAIShown = AiSource == AISource.OpenAI;
        IsHuggingFaceShown = AiSource == AISource.HuggingFace;
    }

    private string GetPluginFolder()
    {
        var pluginFolder = _settingsToolkit.ReadLocalSetting(SettingNames.PluginFolderPath, string.Empty);
        if (string.IsNullOrEmpty(pluginFolder))
        {
            pluginFolder = Path.Combine(ApplicationData.Current.LocalFolder.Path, WorkflowConstants.DefaultPluginFolderName);
        }

        return pluginFolder;
    }

    private async Task<bool> VerifyUserAsync()
    {
        var authTip = _resourceToolkit.GetLocalizedString(StringNames.VerifyIdentity);
        var hwnd = WindowNative.GetWindowHandle(_appViewModel.MainWindow);
        var authResult = await UserConsentVerifierInterop.RequestVerificationForWindowAsync(hwnd, authTip);
        if (authResult != UserConsentVerificationResult.Verified)
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.CancelBecauseNotVerified), InfoType.Warning);
        }

        return authResult == UserConsentVerificationResult.Verified;
    }

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

    private void WriteSecureSetting(SettingNames name, string text)
        => _settingsToolkit.SaveSecureString(name, text);

    partial void OnDefaultConversationTypeChanged(ConversationType value)
        => WriteSetting(SettingNames.LastConversationType, value);

    partial void OnAiSourceChanged(AISource value)
    {
        WriteSetting(SettingNames.AISource, value);
        CheckAISource();
    }

    partial void OnAzureOpenAIAccessKeyChanged(string value)
        => WriteSecureSetting(SettingNames.AzureOpenAIAccessKey, value);

    partial void OnAzureOpenAIChatModelNameChanged(string value)
        => WriteSetting(SettingNames.AzureOpenAIChatModelName, value);

    partial void OnAzureOpenAIEmbeddingModelNameChanged(string value)
        => WriteSetting(SettingNames.AzureOpenAIEmbeddingModelName, value);

    partial void OnAzureOpenAICompletionModelNameChanged(string value)
        => WriteSetting(SettingNames.AzureOpenAICompletionModelName, value);

    partial void OnAzureOpenAIEndpointChanged(string value)
        => WriteSetting(SettingNames.AzureOpenAIEndpoint, value);

    partial void OnOpenAIAccessKeyChanged(string value)
        => WriteSecureSetting(SettingNames.OpenAIAccessKey, value);

    partial void OnOpenAIChatModelNameChanged(string value)
        => WriteSetting(SettingNames.OpenAIChatModelName, value);

    partial void OnOpenAIEmbeddingModelNameChanged(string value)
        => WriteSetting(SettingNames.OpenAIEmbeddingModelName, value);

    partial void OnOpenAICompletionModelNameChanged(string value)
        => WriteSetting(SettingNames.OpenAICompletionModelName, value);

    partial void OnOpenAIOrganizationChanged(string value)
        => WriteSetting(SettingNames.OpenAIOrganization, value);

    partial void OnHuggingFaceAccessKeyChanged(string value)
        => WriteSecureSetting(SettingNames.HuggingFaceAccessKey, value);

    partial void OnHuggingFaceEmbeddingModelNameChanged(string value)
        => WriteSetting(SettingNames.HuggingFaceEmbeddingModelName, value);

    partial void OnHuggingFaceCompletionModelNameChanged(string value)
        => WriteSetting(SettingNames.HuggingFaceCompletionModelName, value);

    partial void OnHuggingFaceEmbeddingEndpointChanged(string value)
        => WriteSetting(SettingNames.HuggingFaceEmbeddingEndpoint, value);

    partial void OnHuggingFaceCompletionEndpointChanged(string value)
        => WriteSetting(SettingNames.HuggingFaceCompletionEndpoint, value);

    partial void OnAzureVoiceKeyChanged(string value)
        => WriteSecureSetting(SettingNames.AzureVoiceKey, value);

    partial void OnAzureVoiceRegionChanged(string value)
        => WriteSetting(SettingNames.AzureVoiceRegion, value);

    partial void OnAzureTranslateKeyChanged(string value)
        => WriteSecureSetting(SettingNames.AzureTranslateKey, value);

    partial void OnAzureTranslateRegionChanged(string value)
        => WriteSetting(SettingNames.AzureTranslateRegion, value);

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

    partial void OnContextResponseTokenLengthChanged(int value)
        => WriteSetting(SettingNames.ContextResponseTokenLength, value);
}
