// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;
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
        IAppViewModel appViewModel,
        ILogger<SettingsPageViewModel> logger)
    {
        _settingsToolkit = settingsToolkit;
        _resourceToolkit = resourceToolkit;
        _fileToolkit = fileToolkit;
        _appViewModel = appViewModel;
        _logger = logger;
        BuildYear = 2023;

        ChatConnectors = new ObservableCollection<IConnectorConfigViewModel>();
        TextCompletionConnectors = new ObservableCollection<IConnectorConfigViewModel>();
        EmbeddingConnectors = new ObservableCollection<IConnectorConfigViewModel>();
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

        ConnectorFolderPath = GetConnectorFolder();

        IsChatEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsChatEnabled, true);
        IsImageEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsImageEnabled, true);
        IsVoiceEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsVoiceEnabled, true);
        IsTranslateEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsTranslateEnabled, true);
        IsStorageEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsStorageEnabled, true);
        IsKnowledgeEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsKnowledgeEnabled, true);
        IsRestartRequest = false;

        CheckAISource();
        CheckTranslateSource();
        VerifyConnectors();
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

    [RelayCommand]
    private async Task ChangeConnectorFolderAsync()
    {
        var folderObj = await _fileToolkit.PickFolderAsync(_appViewModel.MainWindow);
        if (folderObj is not StorageFolder folder)
        {
            return;
        }

        var sourceFolder = GetConnectorFolder();
        if (sourceFolder == folder.Path)
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.CanNotSelectSameFolder), InfoType.Error);
            return;
        }

        if (!Directory.GetFiles(folder.Path).Any())
        {
            var connectors = Directory.GetDirectories(sourceFolder);
            var tasks = new List<Task>();
            foreach (var connectorFolder in connectors)
            {
                tasks.Add(Task.Run(() =>
                {
                    var tempFolder = connectorFolder;
                    var connectorName = new DirectoryInfo(tempFolder).Name;
                    var destPath = Path.Combine(folder.Path, connectorName);
                    if (Directory.Exists(destPath))
                    {
                        Directory.Delete(destPath, true);
                    }

                    Directory.Move(tempFolder, destPath);
                }));
            }

            await Task.WhenAll(tasks);
        }

        _settingsToolkit.WriteLocalSetting(SettingNames.ConnectorFolderPath, folder.Path);
        ConnectorFolderPath = folder.Path;
        _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.MoveCompleted), InfoType.Success);
    }

    [RelayCommand]
    private async Task OpenConnectorFolderAsync()
    {
        var folderPath = GetConnectorFolder();
        var folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
        await Launcher.LaunchFolderAsync(folder);
    }

    [RelayCommand]
    private async Task ImportConnectorAsync()
    {
        var cacheToolkit = Locator.Current.GetService<ICacheToolkit>();
        var fileObj = await _fileToolkit.PickFileAsync(ConnectorConstants.ConnectorExtension, _appViewModel.MainWindow);
        if (fileObj is not StorageFile file)
        {
            return;
        }

        try
        {
            var config = await cacheToolkit.GetConnectorConfigFromZipAsync(file.Path);

            if (string.IsNullOrEmpty(config.Name))
            {
                throw new ArgumentException(_resourceToolkit.GetLocalizedString(StringNames.MustHaveNameOrDescription));
            }

            if (string.IsNullOrEmpty(config.Id))
            {
                throw new ArgumentException(_resourceToolkit.GetLocalizedString(StringNames.MustHaveId));
            }

            if (string.IsNullOrEmpty(config.ExecuteName))
            {
                throw new ArgumentException(_resourceToolkit.GetLocalizedString(StringNames.MustHaveExecuteFile));
            }

            if (config.Features == null || config.Features.Count == 0)
            {
                throw new ArgumentException(_resourceToolkit.GetLocalizedString(StringNames.MustHaveFeature));
            }

            foreach (var feature in config.Features)
            {
                if (string.IsNullOrEmpty(feature.Type))
                {
                    throw new ArgumentException(_resourceToolkit.GetLocalizedString(StringNames.MustHaveType));
                }

                if (feature.Endpoints == null || feature.Endpoints.Count == 0)
                {
                    throw new ArgumentException(_resourceToolkit.GetLocalizedString(StringNames.FeatureMustHaveEndpoints));
                }
            }

            await cacheToolkit.ImportConnectorConfigAsync(config, file.Path);
            _appViewModel.RefreshConnectorsCommand.Execute(false);
            VerifyConnectors();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(ImportConnectorAsync));
            _appViewModel.ShowTip(ex.Message, InfoType.Error);
        }
    }

    [RelayCommand]
    private void RefreshConnector()
    {
        _appViewModel.RefreshConnectorsCommand.Execute(true);
    }

    private void CheckAISource()
    {
        IsAzureOpenAIShown = AiSource == AISource.Azure;
        IsOpenAIShown = AiSource == AISource.OpenAI;
        IsCustomAIShown = AiSource == AISource.Custom;
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

    private string GetConnectorFolder()
    {
        var connectorFolder = _settingsToolkit.ReadLocalSetting(SettingNames.ConnectorFolderPath, string.Empty);
        if (string.IsNullOrEmpty(connectorFolder))
        {
            connectorFolder = Path.Combine(ApplicationData.Current.LocalFolder.Path, ConnectorConstants.DefaultConnectorFolderName);
        }

        return connectorFolder;
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

    private void VerifyConnectors()
    {
        var chatConnectors = _appViewModel.Connectors.Where(p => p.GetData().Features.Any(p => p.Type == ConnectorConstants.ChatType));
        var textConnectors = _appViewModel.Connectors.Where(p => p.GetData().Features.Any(p => p.Type == ConnectorConstants.TextCompletionType));
        var embeddingConnectors = _appViewModel.Connectors.Where(p => p.GetData().Features.Any(p => p.Type == ConnectorConstants.EmbeddingType));

        var currentChatId = _settingsToolkit.ReadLocalSetting(SettingNames.CustomChatConnectorId, string.Empty);
        var currentTextId = _settingsToolkit.ReadLocalSetting(SettingNames.CustomTextCompletionConnectorId, string.Empty);
        var currentEmbeddingId = _settingsToolkit.ReadLocalSetting(SettingNames.CustomEmbeddingConnectorId, string.Empty);

        TryClear(ChatConnectors);
        TryClear(TextCompletionConnectors);
        TryClear(EmbeddingConnectors);

        foreach (var item in chatConnectors)
        {
            ChatConnectors.Add(item);
        }

        foreach (var item in textConnectors)
        {
            TextCompletionConnectors.Add(item);
        }

        foreach (var item in embeddingConnectors)
        {
            EmbeddingConnectors.Add(item);
        }

        SelectedChatConnector = ChatConnectors.FirstOrDefault(p => p.Id == currentChatId) ?? ChatConnectors.FirstOrDefault();
        SelectedTextCompletionConnector = TextCompletionConnectors.FirstOrDefault(p => p.Id == currentTextId) ?? TextCompletionConnectors.FirstOrDefault();
        SelectedEmbeddingConnector = EmbeddingConnectors.FirstOrDefault(p => p.Id == currentEmbeddingId) ?? EmbeddingConnectors.FirstOrDefault();
    }
}
