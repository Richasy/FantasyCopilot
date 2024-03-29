﻿// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using RichasyAssistant.Models.App.Web;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Civitai image view model.
/// </summary>
public sealed partial class CivitaiImageViewModel
{
    private readonly IFileToolkit _fileToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IAppViewModel _appViewModel;
    private readonly ILogger<CivitaiImageViewModel> _logger;
    private string _imageId;
    private CivitaiImage _source;
    private bool _isSaving;

    [ObservableProperty]
    private string _imagePath;

    [ObservableProperty]
    private string _userName;

    [ObservableProperty]
    private bool _hasMetadata;

    [ObservableProperty]
    private string _prompt;

    [ObservableProperty]
    private string _negativePrompt;

    [ObservableProperty]
    private string _sampler;

    [ObservableProperty]
    private string _model;

    [ObservableProperty]
    private double _cfgScale;

    [ObservableProperty]
    private int _steps;

    [ObservableProperty]
    private long _seed;

    [ObservableProperty]
    private int _clipSkip;

    [ObservableProperty]
    private string _createTime;
}
