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
/// View model of the saved sessions page.
/// </summary>
public sealed partial class SavedSessionsModuleViewModel : ViewModelBase, ISavedSessionsModuleViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SavedSessionsModuleViewModel"/> class.
    /// </summary>
    public SavedSessionsModuleViewModel(
        ICacheToolkit cacheToolkit,
        IAppViewModel appViewModel)
    {
        _cacheToolkit = cacheToolkit;
        _appViewModel = appViewModel;
        Sessions = new ObservableCollection<SessionMetadata>();
        Sessions.CollectionChanged += OnSessionsCollectionChanged;
        _cacheToolkit.SessionListChanged += OnSessionListChanged;
        AttachIsRunningToAsyncCommand(p => p = IsLoading = p, InitializeCommand);
        CheckIsEmpty();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (IsLoading || _isInitialized)
        {
            return;
        }

        TryClear(Sessions);
        var sessions = await _cacheToolkit.GetSessionListAsync();
        foreach (var session in sessions)
        {
            Sessions.Add(session);
        }

        _isInitialized = true;
    }

    [RelayCommand]
    private async Task OpenSessionAsync(SessionMetadata metadata)
    {
        var session = await _cacheToolkit.GetSessionByIdAsync(metadata.Id);
        var vm = Locator.Current.GetService<ISessionViewModel>();
        vm.Initialize(session, metadata);
        _appViewModel.Navigate(PageType.ChatSession, vm);
    }

    private void CheckIsEmpty()
        => IsEmpty = Sessions.Count == 0;

    private void OnSessionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => CheckIsEmpty();

    private void OnSessionListChanged(object sender, EventArgs e)
    {
        _isInitialized = false;
        InitializeCommand.Execute(default);
    }
}
