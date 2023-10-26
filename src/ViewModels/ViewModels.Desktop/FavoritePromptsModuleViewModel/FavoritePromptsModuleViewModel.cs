// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Gpt;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Favorite prompts module view model.
/// </summary>
public sealed partial class FavoritePromptsModuleViewModel : ViewModelBase, IFavoritePromptsModuleViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FavoritePromptsModuleViewModel"/> class.
    /// </summary>
    public FavoritePromptsModuleViewModel(
        ICacheToolkit cacheToolkit,
        IResourceToolkit resourceToolkit,
        IAppViewModel appViewModel)
    {
        _cacheToolkit = cacheToolkit;
        _resourceToolkit = resourceToolkit;
        _appViewModel = appViewModel;
        Prompts = new SynchronizedObservableCollection<SessionMetadata>();
        Prompts.CollectionChanged += OnPromptsCollectionChanged;
        _cacheToolkit.PromptListChanged += OnPromptListChanged;
        AttachIsRunningToAsyncCommand(p => IsLoading = p, InitializeCommand);
        CheckIsEmpty();
    }

    [RelayCommand]
    private void CreateSession(SessionMetadata metadata)
    {
        var copyName = metadata.Name;
        var copyDescription = metadata.Description;
        var copyPrompt = metadata.SystemPrompt;
        var newData = new SessionMetadata
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = copyName,
            Description = copyDescription,
            SystemPrompt = copyPrompt,
        };

        var vm = Locator.Current.GetService<ISessionViewModel>();
        vm.Initialize(default, newData);
        _appViewModel.Navigate(PageType.ChatSession, vm);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (IsLoading || _isInitialized)
        {
            return;
        }

        TryClear(Prompts);

        var prompts = await _cacheToolkit.GetCustomPromptsAsync();
        foreach (var session in prompts)
        {
            Prompts.Add(session);
        }

        _isInitialized = true;
    }

    [RelayCommand]
    private async Task ImportAsync()
    {
        var result = await _cacheToolkit.ImportPromptsAsync();
        if (result == null)
        {
            return;
        }
        else if (result.Value)
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.DataImported), InfoType.Success);
        }
        else
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.ImportDataFailed), InfoType.Error);
        }
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        if (Prompts.Count == 0)
        {
            return;
        }

        var result = await _cacheToolkit.ExportPromptsAsync();
        if (result == null)
        {
            return;
        }
        else if (result.Value)
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.DataExported), InfoType.Success);
        }
        else
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.ExportDataFailed), InfoType.Error);
        }
    }

    private void CheckIsEmpty()
        => IsEmpty = Prompts.Count == 0;

    private void OnPromptsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => CheckIsEmpty();

    private void OnPromptListChanged(object sender, EventArgs e)
    {
        _isInitialized = false;
        InitializeCommand.Execute(default);
    }
}
