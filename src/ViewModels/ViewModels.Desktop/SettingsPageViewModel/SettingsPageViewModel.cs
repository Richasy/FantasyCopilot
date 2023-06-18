// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.DI.Container;
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
        TranslateSource = _settingsToolkit.ReadLocalSetting(SettingNames.TranslateSource, TranslateSource.Azure);

        AzureOpenAIAccessKey = _settingsToolkit.RetrieveSecureString(SettingNames.AzureOpenAIAccessKey);
        AzureOpenAIChatModelName = _settingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAIChatModelName, string.Empty);
        AzureOpenAICompletionModelName = _settingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAICompletionModelName, string.Empty);
        AzureOpenAIEmbeddingModelName = _settingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAIEmbeddingModelName, string.Empty);
        AzureOpenAIEndpoint = _settingsToolkit.ReadLocalSetting(SettingNames.AzureOpenAIEndpoint, string.Empty);

        OpenAIAccessKey = _settingsToolkit.RetrieveSecureString(SettingNames.OpenAIAccessKey);
        OpenAIOrganization = _settingsToolkit.ReadLocalSetting(SettingNames.OpenAIOrganization, string.Empty);
        OpenAICustomEndpoint = _settingsToolkit.ReadLocalSetting(SettingNames.OpenAICustomEndpoint, string.Empty);
        OpenAIChatModelName = _settingsToolkit.ReadLocalSetting(SettingNames.OpenAIChatModelName, string.Empty);
        OpenAICompletionModelName = _settingsToolkit.ReadLocalSetting(SettingNames.OpenAICompletionModelName, string.Empty);
        OpenAIEmbeddingModelName = _settingsToolkit.ReadLocalSetting(SettingNames.OpenAIEmbeddingModelName, string.Empty);

        MaxSplitContentLength = _settingsToolkit.ReadLocalSetting(SettingNames.MaxSplitContentLength, 1024);
        MaxParagraphTokenLength = _settingsToolkit.ReadLocalSetting(SettingNames.MaxParagraphTokenLength, 512);
        ContextLimit = _settingsToolkit.ReadLocalSetting(SettingNames.ContextLimit, 3);
        MinRelevanceScore = _settingsToolkit.ReadLocalSetting(SettingNames.ContextMinRelevanceScore, 0.7d);

        AzureVoiceKey = _settingsToolkit.RetrieveSecureString(SettingNames.AzureVoiceKey);
        AzureVoiceRegion = _settingsToolkit.ReadLocalSetting(SettingNames.AzureVoiceRegion, string.Empty);

        AzureTranslateKey = _settingsToolkit.RetrieveSecureString(SettingNames.AzureTranslateKey);
        AzureTranslateRegion = _settingsToolkit.ReadLocalSetting(SettingNames.AzureTranslateRegion, string.Empty);

        BaiduTranslateAppId = _settingsToolkit.RetrieveSecureString(SettingNames.BaiduTranslateAppId);
        BaiduTranslateAppKey = _settingsToolkit.RetrieveSecureString(SettingNames.BaiduTranslateAppKey);

        StableDiffusionUrl = _settingsToolkit.ReadLocalSetting(SettingNames.StableDiffusionUrl, string.Empty);

        OpenConsoleWhenPluginRunning = _settingsToolkit.ReadLocalSetting(SettingNames.ShowConsoleWhenPluginRunning, false);
        HideWhenCloseWindow = _settingsToolkit.ReadLocalSetting(SettingNames.HideWhenCloseWindow, false);
        MessageUseMarkdown = _settingsToolkit.ReadLocalSetting(SettingNames.MessageUseMarkdown, true);

        IsChatEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsChatEnabled, true);
        IsImageEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsImageEnabled, true);
        IsVoiceEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsVoiceEnabled, true);
        IsTranslateEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsTranslateEnabled, true);
        IsStorageEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsStorageEnabled, true);
        IsKnowledgeEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsKnowledgeEnabled, true);
        IsRestartRequest = false;

        CheckAISource();
        CheckTranslateSource();
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
    }

    private void CheckTranslateSource()
    {
        IsAzureTranslateShown = TranslateSource == TranslateSource.Azure;
        IsBaiduTranslateShown = TranslateSource == TranslateSource.Baidu;
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
}
