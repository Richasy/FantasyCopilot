// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;

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
        ILogger<AppViewModel> logger)
    {
        _logger = logger;
        _resourceToolkit = resourceToolkit;
        _settingsToolkit = settingsToolkit;
        NavigateItems = new ObservableCollection<NavigateItem>();
    }

    /// <inheritdoc/>
    public void Initialize()
    {
        var isSkipWelcome = _settingsToolkit.IsSettingKeyExist(SettingNames.IsSkipWelcomeScreen);
        if (!isSkipWelcome)
        {
            Navigate(PageType.Welcome);
        }
        else
        {
            ReloadAllServicesCommand.Execute(default);
            LoadNavItems();
            var lastOpenPage = _settingsToolkit.ReadLocalSetting(SettingNames.LastOpenPageType, PageType.ChatSession);
            if (!NavigateItems.Any(p => p.Type == lastOpenPage))
            {
                lastOpenPage = NavigateItems.First().Type;
            }

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

    partial void OnCurrentNavigateItemChanged(NavigateItem value)
    {
        if (value != null)
        {
            Navigate(value.Type);
        }
    }
}
