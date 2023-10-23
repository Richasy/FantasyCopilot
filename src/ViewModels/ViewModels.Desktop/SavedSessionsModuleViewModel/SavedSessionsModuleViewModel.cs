// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Gpt;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

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
        IResourceToolkit resourceToolkit,
        IAppViewModel appViewModel)
    {
        _cacheToolkit = cacheToolkit;
        _resourceToolkit = resourceToolkit;
        _appViewModel = appViewModel;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        Sessions = new SynchronizedObservableCollection<SessionMetadata>();
        Sessions.CollectionChanged += OnSessionsCollectionChanged;
        _cacheToolkit.SessionListChanged += OnSessionListChanged;
        CheckIsEmpty();
    }

    [RelayCommand]
    private void Initialize()
    {
        _dispatcherQueue.TryEnqueue(async () =>
        {
            if (IsLoading || _isInitialized)
            {
                return;
            }

            IsLoading = true;
            TryClear(Sessions);
            var sessions = await _cacheToolkit.GetSessionListAsync();
            foreach (var session in sessions)
            {
                Sessions.Add(session);
            }

            _isInitialized = true;
            IsLoading = false;
        });
    }

    [RelayCommand]
    private async Task OpenSessionAsync(SessionMetadata metadata)
    {
        var session = await _cacheToolkit.GetSessionByIdAsync(metadata.Id);
        var vm = Locator.Current.GetService<ISessionViewModel>();
        vm.Initialize(session, metadata);
        _appViewModel.Navigate(PageType.ChatSession, vm);
    }

    [RelayCommand]
    private async Task ImportAsync()
    {
        var result = await _cacheToolkit.ImportSessionsAsync();
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
        var result = await _cacheToolkit.ExportSessionsAsync();
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
        => IsEmpty = Sessions.Count == 0;

    private void OnSessionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => CheckIsEmpty();

    private void OnSessionListChanged(object sender, EventArgs e)
    {
        _isInitialized = false;
        InitializeCommand.Execute(default);
    }
}
