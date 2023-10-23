// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Files;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.ViewModels.Interfaces;

/// <summary>
/// Storage page view model.
/// </summary>
public interface IStoragePageViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Search keyword.
    /// </summary>
    string GenericKeyword { get; set; }

    /// <summary>
    /// File search keyword.
    /// </summary>
    string FileKeyword { get; set; }

    /// <summary>
    /// Audio search keyword.
    /// </summary>
    string AudioKeyword { get; set; }

    /// <summary>
    /// Whether searching or not.
    /// </summary>
    bool IsSearching { get; }

    /// <summary>
    /// Whether the search results are empty.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Whether to display the initial tip.
    /// </summary>
    bool IsInitialTipVisible { get; }

    /// <summary>
    /// Storage search type.
    /// </summary>
    StorageSearchType SearchType { get; set; }

    /// <summary>
    /// The bit depth of the current picture query.
    /// </summary>
    ImageColorDepth CurrentImageColorDepth { get; set; }

    /// <summary>
    /// The orientation of the current picture query.
    /// </summary>
    ImageOrientation CurrentImageOrientation { get; set; }

    /// <summary>
    /// File sort type.
    /// </summary>
    FileSortType SortType { get; set; }

    /// <summary>
    /// File count.
    /// </summary>
    int FileCount { get; }

    /// <summary>
    /// Width of the current picture query.
    /// </summary>
    double CurrentImageWidth { get; set; }

    /// <summary>
    /// Height of the current picture query.
    /// </summary>
    double CurrentImageHeight { get; set; }

    /// <summary>
    /// Is it a generic search.
    /// </summary>
    bool IsGenericSearch { get; }

    /// <summary>
    /// Is it a file search.
    /// </summary>
    bool IsFileSearch { get; }

    /// <summary>
    /// Is it a audio search.
    /// </summary>
    bool IsAudioSearch { get; }

    /// <summary>
    /// Is it a image search.
    /// </summary>
    bool IsImageSearch { get; }

    /// <summary>
    /// Current file search type.
    /// </summary>
    FileSearchEntry CurrentFileSearchType { get; set; }

    /// <summary>
    /// Current audio search type.
    /// </summary>
    AudioSearchEntry CurrentAudioSearchType { get; set; }

    /// <summary>
    /// Search results.
    /// </summary>
    SynchronizedObservableCollection<FileItem> Items { get; }

    /// <summary>
    /// File search selections.
    /// </summary>
    SynchronizedObservableCollection<FileSearchEntry> FileSearchEntries { get; }

    /// <summary>
    /// File search selections.
    /// </summary>
    SynchronizedObservableCollection<AudioSearchEntry> AudioSearchEntries { get; }

    /// <summary>
    /// Search files.
    /// </summary>
    IAsyncRelayCommand SearchCommand { get; }

    /// <summary>
    /// Open item.
    /// </summary>
    IRelayCommand<FileItem> OpenCommand { get; }

    /// <summary>
    /// Open in file explorer.
    /// </summary>
    IRelayCommand<FileItem> OpenInFileExplorerCommand { get; }

    /// <summary>
    /// Copy file path.
    /// </summary>
    IRelayCommand<FileItem> CopyPathCommand { get; }

    /// <summary>
    /// Use another app open it.
    /// </summary>
    IAsyncRelayCommand<FileItem> OpenWithCommand { get; }

    /// <summary>
    /// Get current search keyword.
    /// </summary>
    /// <returns>Keyword.</returns>
    string GetKeyword();
}
