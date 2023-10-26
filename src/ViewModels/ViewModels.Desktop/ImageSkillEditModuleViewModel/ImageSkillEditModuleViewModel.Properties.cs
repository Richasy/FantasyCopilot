// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using RichasyAssistant.Models.App.Image;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Image skill edit module view model.
/// </summary>
public sealed partial class ImageSkillEditModuleViewModel
{
    private readonly ICacheToolkit _cacheToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IAppViewModel _appViewModel;
    private GenerateOptions _sourceConfig;

    private string _id;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private string _prompt;

    [ObservableProperty]
    private string _negativePrompt;

    [ObservableProperty]
    private bool _isInitializing;

    [ObservableProperty]
    private bool _isDisconnected;

    [ObservableProperty]
    private IImageGenerateOptionsViewModel _options;
}
