// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Knowledge;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Windows.Storage;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Knowledge page view model.
/// </summary>
public sealed partial class KnowledgePageViewModel : ViewModelBase, IKnowledgePageViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgePageViewModel"/> class.
    /// </summary>
    public KnowledgePageViewModel(
        ICacheToolkit cacheToolkit,
        IResourceToolkit resourceToolkit,
        IMemoryService memoryService,
        IAppViewModel appViewModel,
        IKnowledgeBaseSessionViewModel knowledgeBaseSessionViewModel,
        ILogger<KnowledgePageViewModel> logger)
    {
        _cacheToolkit = cacheToolkit;
        _resourceToolkit = resourceToolkit;
        _memoryService = memoryService;
        _appViewModel = appViewModel;
        _knowledgeBaseSessionViewModel = knowledgeBaseSessionViewModel;
        _logger = logger;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _progressTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200),
        };

        _progressTimer.Tick += OnProgressTimerTick;
        Bases = new ObservableCollection<IKnowledgeBaseItemViewModel>();
        Bases.CollectionChanged += OnBasesCollectionChanged;
        _cacheToolkit.KnowledgeBaseListChanged += OnKnowledgeBaseListChangedAsync;
        CheckIsEmpty();

        AttachIsRunningToAsyncCommand(p => IsLoading = p, InitializeCommand);
        AttachIsRunningToAsyncCommand(p => IsBaseCreating = p, CreateBaseCommand, ImportBaseCommand, ImportFolderCommand, ImportFileCommand);
        AttachIsRunningToAsyncCommand(p => IsBaseConnecting = p, EnterBaseCommand);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (IsLoading || _isInitialized)
        {
            return;
        }

        TryClear(Bases);

        var bases = await _cacheToolkit.GetKnowledgeBasesAsync();
        foreach (var item in bases)
        {
            var vm = Locator.Current.GetService<IKnowledgeBaseItemViewModel>();
            vm.InjectData(item);
            Bases.Add(vm);
        }

        _isInitialized = true;
    }

    [RelayCommand]
    private async Task ImportBaseAsync(KnowledgeBase data)
    {
        var hasSameBase = Bases.Any(p => p.GetData().DatabasePath.Equals(data.DatabasePath));
        if (hasSameBase)
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.AlreadyHaveSameKnowledgeBase), InfoType.Error);
            return;
        }

        await _cacheToolkit.AddOrUpdateKnowledgeBaseAsync(data);
    }

    [RelayCommand]
    private async Task CreateBaseAsync(BaseCreation data)
    {
        var blankDb = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/blank.db")).AsTask();
        var targetDb = await StorageFile.GetFileFromPathAsync(data.DatabasePath).AsTask();
        await blankDb.CopyAndReplaceAsync(targetDb);
        var isConnected = await _memoryService.ConnectSQLiteKnowledgeBaseAsync(data.DatabasePath);
        if (!isConnected)
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.ConnectDatabaseFailed), InfoType.Error);
            return;
        }

        _progressTimer.Start();
        try
        {
            var tip = string.Empty;
            var isWarning = false;
            if (!string.IsNullOrEmpty(data.FolderPath))
            {
                var (totalCount, failedCount) = await _memoryService.ImportFolderToMemoryAsync(data.FolderPath, data.SearchPattern);
                if (failedCount > 0)
                {
                    var text = _resourceToolkit.GetLocalizedString(StringNames.ImportFolderNotFullySucceed);
                    tip = string.Format(text, failedCount, totalCount);
                    isWarning = true;
                }
                else
                {
                    tip = _resourceToolkit.GetLocalizedString(StringNames.ImportFolderSucceed);
                }
            }
            else if (!string.IsNullOrEmpty(data.FilePath))
            {
                var isSuccess = await _memoryService.ImportFileToMemoryAsync(data.FilePath);
                tip = isSuccess
                    ? _resourceToolkit.GetLocalizedString(StringNames.ImportFileSucceed)
                    : _resourceToolkit.GetLocalizedString(StringNames.ImportFileFailed);
                isWarning = !isSuccess;
            }

            if (!string.IsNullOrEmpty(tip))
            {
                var infoType = isWarning ? InfoType.Warning : InfoType.Success;
                _appViewModel.ShowTip(tip, infoType);
            }

            var baseData = new KnowledgeBase
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = data.Name,
                DatabasePath = data.DatabasePath,
            };

            await _cacheToolkit.AddOrUpdateKnowledgeBaseAsync(baseData);
            EnterBaseCommand.Execute(baseData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(CreateBaseCommand));
            _appViewModel.ShowTip(ex.Message, InfoType.Error);
        }

        _progressTimer.Stop();
    }

    [RelayCommand]
    private async Task ImportFolderAsync(BaseCreation creation)
    {
        try
        {
            var isConnected = await _memoryService.ConnectSQLiteKnowledgeBaseAsync(creation.DatabasePath);
            if (!isConnected)
            {
                _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.ConnectDatabaseFailed), InfoType.Error);
                return;
            }

            _progressTimer.Start();
            var (totalCount, failedCount) = await _memoryService.ImportFolderToMemoryAsync(creation.FolderPath, creation.SearchPattern);
            var isWarning = false;
            var tip = string.Empty;
            if (failedCount > 0)
            {
                var text = _resourceToolkit.GetLocalizedString(StringNames.ImportFolderNotFullySucceed);
                tip = string.Format(text, failedCount, totalCount);
                isWarning = true;
            }
            else
            {
                tip = _resourceToolkit.GetLocalizedString(StringNames.ImportFolderSucceed);
            }

            var infoType = isWarning ? InfoType.Warning : InfoType.Success;
            _appViewModel.ShowTip(tip, infoType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(ImportFolderCommand));
            _appViewModel.ShowTip(ex.Message, InfoType.Error);
        }

        _progressTimer.Stop();
    }

    [RelayCommand]
    private async Task ImportFileAsync(BaseCreation creation)
    {
        try
        {
            var isConnected = await _memoryService.ConnectSQLiteKnowledgeBaseAsync(creation.DatabasePath);
            if (!isConnected)
            {
                _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.ConnectDatabaseFailed), InfoType.Error);
                return;
            }

            var isSuccess = await _memoryService.ImportFileToMemoryAsync(creation.FilePath);
            var tip = isSuccess
                    ? _resourceToolkit.GetLocalizedString(StringNames.ImportFileSucceed)
                    : _resourceToolkit.GetLocalizedString(StringNames.ImportFileFailed);

            var infoType = isSuccess ? InfoType.Success : InfoType.Error;
            _appViewModel.ShowTip(tip, infoType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(ImportFileCommand));
            _appViewModel.ShowTip(ex.Message, InfoType.Error);
        }
    }

    [RelayCommand]
    private Task RemoveBaseAsync(string id)
        => _cacheToolkit.DeleteKnowledgeBaseAsync(id);

    [RelayCommand]
    private Task UpdateBaseAsync(KnowledgeBase data)
        => _cacheToolkit.AddOrUpdateKnowledgeBaseAsync(data);

    [RelayCommand]
    private async Task EnterBaseAsync(KnowledgeBase data)
    {
        var isConnected = await _memoryService.ConnectSQLiteKnowledgeBaseAsync(data.DatabasePath);
        if (!isConnected)
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.ConnectDatabaseFailed), InfoType.Error);
            return;
        }

        CurrentBase = data;

        _knowledgeBaseSessionViewModel.Initialize(data);
        _appViewModel.IsBackButtonShown = true;
    }

    [RelayCommand]
    private void ExitBase()
    {
        _memoryService.DisconnectSQLiteKnowledgeBase();
        CurrentBase = default;
        _appViewModel.IsBackButtonShown = false;
    }

    private void CheckIsEmpty()
        => IsEmpty = Bases.Count == 0;

    private void OnProgressTimerTick(object sender, object e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (!IsBaseCreating)
            {
                return;
            }

            var (importedFiles, totalFiles) = _memoryService.GetImportToMemoryProgress();
            ImportedFileCount = importedFiles;
            TotalFileCount = totalFiles;
        });
    }

    private void OnBasesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => CheckIsEmpty();

    private async void OnKnowledgeBaseListChangedAsync(object sender, EventArgs e)
    {
        _isInitialized = false;
        await InitializeAsync();
    }
}
