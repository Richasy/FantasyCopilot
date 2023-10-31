// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Files;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Libs.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Storage page view model.
/// </summary>
public sealed partial class StoragePageViewModel : ViewModelBase, IStoragePageViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StoragePageViewModel"/> class.
    /// </summary>
    public StoragePageViewModel(
        IStorageService storageService,
        IResourceToolkit resourceToolkit,
        ISettingsToolkit settingsToolkit,
        IAppViewModel appViewModel)
    {
        _storageService = storageService;
        _settingsToolkit = settingsToolkit;
        _resourceToolkit = resourceToolkit;
        _appViewModel = appViewModel;
        IsInitialTipVisible = true;
        Items = new SynchronizedObservableCollection<FileItem>();
        FileSearchEntries = new SynchronizedObservableCollection<FileSearchEntry>();
        AudioSearchEntries = new SynchronizedObservableCollection<AudioSearchEntry>();
        InitializeFileSearchTypes();
        InitializeAudioSearchTypes();
        ResetSearchState();
        AttachIsRunningToAsyncCommand(p => IsSearching = p, SearchCommand);
        SearchType = _settingsToolkit.ReadLocalSetting(SettingNames.LastStorageSearchType, StorageSearchType.Generic);
        SortType = _settingsToolkit.ReadLocalSetting(SettingNames.LastStorageSortType, FileSortType.NameAtoZ);
    }

    /// <inheritdoc/>
    public string GetKeyword()
    {
        return SearchType switch
        {
            StorageSearchType.Generic => GenericKeyword,
            StorageSearchType.File => FileKeyword,
            StorageSearchType.Audio => AudioKeyword,
            StorageSearchType.Image => string.Empty,
            _ => throw new NotImplementedException(),
        };
    }

    [RelayCommand]
    private void Open(FileItem item)
    {
        try
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = item.Path;
            process.Start();
        }
        catch
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.OpenFailed), InfoType.Error);
        }
    }

    [RelayCommand]
    private void OpenInFileExplorer(FileItem item)
    {
        try
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = string.IsNullOrEmpty(item.FileSize) && Directory.Exists(item.Path)
                ? item.Path
                : Directory.GetParent(item.Path).FullName;

            process.Start();
        }
        catch
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.OpenFailed), InfoType.Error);
        }
    }

    [RelayCommand]
    private void CopyPath(FileItem item)
    {
        var dp = new DataPackage();
        dp.SetText(item.Path);
        Clipboard.SetContent(dp);
        _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.PathCopied), InfoType.Success);
    }

    [RelayCommand]
    private async Task OpenWithAsync(FileItem item)
    {
        try
        {
            var file = await StorageFile.GetFileFromPathAsync(item.Path);
            var options = new LauncherOptions()
            {
                DisplayApplicationPicker = true,
            };
            await Launcher.LaunchFileAsync(file, options);
        }
        catch
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.OpenFailed), InfoType.Error);
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        TryClear(Items);
        if (IsSearching)
        {
            return;
        }

        IsInitialTipVisible = false;
        IsEmpty = false;
        FileCount = 0;
        IEnumerable<FileItem> items = default;
        if (IsGenericSearch)
        {
            items = await _storageService.SearchItemsAsync(GenericKeyword);
        }
        else if (IsFileSearch)
        {
            items = await _storageService.SearchFilesAsync(CurrentFileSearchType.Type, FileKeyword);
        }
        else if (IsAudioSearch)
        {
            items = await _storageService.SearchAudiosAsync(CurrentAudioSearchType.Type, AudioKeyword);
        }
        else if (IsImageSearch)
        {
            items = await _storageService.SearchImagesAsync(
                Convert.ToInt32(CurrentImageWidth),
                Convert.ToInt32(CurrentImageHeight),
                CurrentImageOrientation,
                CurrentImageColorDepth);
        }

        if (items != null && items.Count() > 0)
        {
            var list = GetSortedList(items);
            TryClear(Items);
            foreach (var item in list)
            {
                Items.Add(item);
            }
        }

        FileCount = Items.Count;
        IsEmpty = FileCount == 0;
    }
}
