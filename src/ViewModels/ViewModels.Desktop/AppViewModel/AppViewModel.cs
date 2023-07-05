// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Application view model.
/// </summary>
public sealed partial class AppViewModel : ViewModelBase, IAppViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppViewModel"/> class.
    /// </summary>
    public AppViewModel(
        IResourceToolkit resourceToolkit,
        ISettingsToolkit settingsToolkit,
        ICacheToolkit cacheToolkit,
        ILogger<AppViewModel> logger)
    {
        _logger = logger;
        _resourceToolkit = resourceToolkit;
        _settingsToolkit = settingsToolkit;
        _cacheToolkit = cacheToolkit;
        NavigateItems = new SynchronizedObservableCollection<NavigateItem>();

        Connectors = new SynchronizedObservableCollection<IConnectorConfigViewModel>();
        ConnectorGroup = new Dictionary<ConnectorType, IConnectorConfigViewModel>();
    }

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        var isSkipWelcome = _settingsToolkit.IsSettingKeyExist(SettingNames.IsSkipWelcomeScreen);
        if (!isSkipWelcome)
        {
            Navigate(PageType.Welcome);
        }
        else
        {
            LoadNavItems();
            var lastOpenPage = _settingsToolkit.ReadLocalSetting(SettingNames.LastOpenPageType, PageType.ChatSession);
            if (!NavigateItems.Any(p => p.Type == lastOpenPage))
            {
                lastOpenPage = NavigateItems.First().Type;
            }

            await LoadAllConnectorsAsync();
            ReloadAllServices();
            Navigate(lastOpenPage);
        }

        _logger.LogTrace("Application completes initialization");
    }

    /// <inheritdoc/>
    public void Navigate(PageType page, object parameter = null)
    {
        if (CurrentPage == page)
        {
            return;
        }

        _logger.LogTrace($"Navigate {page}");
        NavigateRequest?.Invoke(this, new NavigateEventArgs(page, parameter));
        CurrentPage = page;
        if (CurrentNavigateItem?.Type != page)
        {
            CurrentNavigateItem = NavigateItems.FirstOrDefault(p => p.Type == CurrentPage);
        }

        IsNavigationMenuShown = page != PageType.Welcome && page != PageType.None;
        if (IsNavigationMenuShown
            && page != PageType.Settings)
        {
            _settingsToolkit.WriteLocalSetting(SettingNames.LastOpenPageType, page);
        }
    }

    /// <inheritdoc/>
    public void ShowTip(string message, InfoType type = InfoType.Information)
        => RequestShowTip?.Invoke(this, new AppTipNotificationEventArgs(message, type));

    /// <inheritdoc/>
    public void ShowMessage(string message)
        => RequestShowMessage?.Invoke(this, message);

    [RelayCommand]
    private static void RestartAsAdmin()
    {
        Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().UnregisterKey();
        Application.Current.Exit();
        var process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/c start fancop://";
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.Verb = "runas";
        process.StartInfo.CreateNoWindow = true;
        process.Start();
    }

    [RelayCommand]
    private void Back()
    {
        if (!IsBackButtonShown)
        {
            return;
        }

        BackRequest?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void ReloadAllServices()
    {
        var kernelService = Locator.Current.GetService<IKernelService>();
        var voiceService = Locator.Current.GetService<IVoiceService>();
        var imageService = Locator.Current.GetService<IImageService>();
        var translateService = Locator.Current.GetService<ITranslateService>();
        var storageService = Locator.Current.GetService<IStorageService>();

        kernelService.ReloadConfig();
        voiceService.ReloadConfig();
        imageService.ReloadConfig();
        translateService.ReloadConfig();
        storageService.ReloadConfig();

        IsChatAvailable = kernelService.IsChatSupport;
        IsKnowledgeAvailable = kernelService.IsMemorySupport;
        IsVoiceAvailable = voiceService.HasValidConfig;
        IsImageAvailable = imageService.HasValidConfig;
        IsTranslateAvailable = translateService.HasValidConfig;
        IsStorageAvailable = storageService.HasValidConfig;

        CheckConnectorGroup();
    }

    [RelayCommand]
    private void CheckStorageService()
    {
        var storageService = Locator.Current.GetService<IStorageService>();
        IsStorageAvailable = storageService.HasValidConfig;
    }

    [RelayCommand]
    private void CheckImageService()
    {
        var imageService = Locator.Current.GetService<IImageService>();
        IsImageAvailable = imageService.HasValidConfig;
    }

    [RelayCommand]
    private async Task RefreshConnectorsAsync(bool force = false)
    {
        await _cacheToolkit.InitializeConnectorsAsync(force);
        LoadConnectorsAfterInitialized();
    }

    private void LoadNavItems()
    {
        TryClear(NavigateItems);
        var isChatEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsChatEnabled, true);
        var isImageEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsImageEnabled, true);
        var isVoiceEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsVoiceEnabled, true);
        var isTranslateEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsTranslateEnabled, true);
        var isStorageEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsStorageEnabled, true);
        var isKnowledgeEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.IsKnowledgeEnabled, true);

        if (isChatEnabled)
        {
            NavigateItems.Add(new NavigateItem(_resourceToolkit.GetLocalizedString(StringNames.NewSession), PageType.ChatSession, FluentSymbol.Chat));
            NavigateItems.Add(new NavigateItem(_resourceToolkit.GetLocalizedString(StringNames.PromptsAndSessions), PageType.PromptsAndSession, FluentSymbol.Channel));
        }

        if (isKnowledgeEnabled)
        {
            NavigateItems.Add(new NavigateItem(_resourceToolkit.GetLocalizedString(StringNames.KnowledgeBase), PageType.Knowledge, FluentSymbol.BrainCircuit));
        }

        if (isImageEnabled)
        {
            NavigateItems.Add(new NavigateItem(_resourceToolkit.GetLocalizedString(StringNames.ImageService), PageType.Image, FluentSymbol.Image));
        }

        if (isVoiceEnabled)
        {
            NavigateItems.Add(new NavigateItem(_resourceToolkit.GetLocalizedString(StringNames.VoiceService), PageType.Voice, FluentSymbol.SpeakerEdit));
        }

        if (isTranslateEnabled)
        {
            NavigateItems.Add(new NavigateItem(_resourceToolkit.GetLocalizedString(StringNames.Translate), PageType.Translate, FluentSymbol.Translate));
        }

        if (isStorageEnabled)
        {
            NavigateItems.Add(new NavigateItem(_resourceToolkit.GetLocalizedString(StringNames.StorageSearch), PageType.StorageSearch, FluentSymbol.DocumentSearch));
        }

        NavigateItems.Add(new NavigateItem(_resourceToolkit.GetLocalizedString(StringNames.Workspace), PageType.Workspace, FluentSymbol.Beaker));
    }

    private async Task LoadAllConnectorsAsync()
    {
        await _cacheToolkit.InitializeConnectorsAsync();
        LoadConnectorsAfterInitialized();
    }

    private void LoadConnectorsAfterInitialized()
    {
        var connectors = _cacheToolkit.GetConnectors();
        TryClear(Connectors);
        if (connectors.Any())
        {
            foreach (var connector in connectors)
            {
                var vm = Locator.Current.GetService<IConnectorConfigViewModel>();
                vm.InjectData(connector);
                Connectors.Add(vm);
            }
        }
    }

    private void CheckConnectorGroup()
    {
        var isCustomConnectorEnabled = _settingsToolkit.ReadLocalSetting(SettingNames.AISource, AISource.Azure) == AISource.Custom;
        if (!isCustomConnectorEnabled)
        {
            if (ConnectorGroup.Any())
            {
                foreach (var connector in ConnectorGroup)
                {
                    connector.Value.ExitCommand.Execute(default);
                }

                ConnectorGroup.Clear();
            }

            return;
        }

        CheckAndAddConnector(SettingNames.CustomChatConnectorId, ConnectorType.Chat);
        CheckAndAddConnector(SettingNames.CustomTextCompletionConnectorId, ConnectorType.TextCompletion);
        CheckAndAddConnector(SettingNames.CustomEmbeddingConnectorId, ConnectorType.Embedding);

        // foreach (var item in ConnectorGroup)
        // {
        //    if (item.Value.IsLaunched)
        //    {
        //        continue;
        //    }
        //    item.Value.LaunchCommand.Execute(default);
        // }
        void CheckAndAddConnector(SettingNames connectorIdSetting, ConnectorType connectorType)
        {
            var connectorId = _settingsToolkit.ReadLocalSetting(connectorIdSetting, string.Empty);
            var connector = Connectors.FirstOrDefault(p => p.Id == connectorId);
            if (ConnectorGroup.ContainsKey(connectorType))
            {
                var source = ConnectorGroup[connectorType];
                if (source.Id != connectorId)
                {
                    source.ExitCommand.Execute(default);
                    ConnectorGroup.Remove(connectorType);
                    if (connector != null)
                    {
                        ConnectorGroup.Add(connectorType, connector);
                    }
                }
            }
            else if (connector != null)
            {
                ConnectorGroup.Add(connectorType, connector);
            }
        }
    }

    partial void OnCurrentNavigateItemChanged(NavigateItem value)
    {
        if (value != null)
        {
            Navigate(value.Type);
        }
    }
}
