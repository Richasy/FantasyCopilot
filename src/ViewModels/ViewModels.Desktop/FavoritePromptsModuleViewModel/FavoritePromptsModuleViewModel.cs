// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

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
        Prompts = new ObservableCollection<SessionMetadata>();
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
