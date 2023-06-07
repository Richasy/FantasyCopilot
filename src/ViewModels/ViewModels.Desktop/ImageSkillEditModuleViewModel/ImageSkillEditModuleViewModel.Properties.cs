// Copyright (c) Fantasy Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App.Image;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

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
