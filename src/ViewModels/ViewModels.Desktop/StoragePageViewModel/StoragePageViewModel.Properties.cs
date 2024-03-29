﻿// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Files;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Storage page view model.
/// </summary>
public sealed partial class StoragePageViewModel
{
    private readonly IStorageService _storageService;
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IAppViewModel _appViewModel;

    [ObservableProperty]
    private string _genericKeyword;

    [ObservableProperty]
    private string _fileKeyword;

    [ObservableProperty]
    private string _audioKeyword;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isInitialTipVisible;

    [ObservableProperty]
    private StorageSearchType _searchType;

    [ObservableProperty]
    private ImageColorDepth _currentImageColorDepth;

    [ObservableProperty]
    private ImageOrientation _currentImageOrientation;

    [ObservableProperty]
    private FileSortType _sortType;

    [ObservableProperty]
    private int _fileCount;

    [ObservableProperty]
    private double _currentImageWidth;

    [ObservableProperty]
    private double _currentImageHeight;

    [ObservableProperty]
    private FileSearchEntry _currentFileSearchType;

    [ObservableProperty]
    private AudioSearchEntry _currentAudioSearchType;

    [ObservableProperty]
    private bool _isGenericSearch;

    [ObservableProperty]
    private bool _isFileSearch;

    [ObservableProperty]
    private bool _isAudioSearch;

    [ObservableProperty]
    private bool _isImageSearch;

    /// <inheritdoc/>
    public SynchronizedObservableCollection<FileItem> Items { get; }

    /// <inheritdoc/>
    public SynchronizedObservableCollection<FileSearchEntry> FileSearchEntries { get; }

    /// <inheritdoc/>
    public SynchronizedObservableCollection<AudioSearchEntry> AudioSearchEntries { get; }
}
