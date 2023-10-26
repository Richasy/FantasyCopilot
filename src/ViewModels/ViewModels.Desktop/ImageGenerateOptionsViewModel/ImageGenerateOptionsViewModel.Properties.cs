// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Image generate options view model.
/// </summary>
public sealed partial class ImageGenerateOptionsViewModel
{
    private readonly ISettingsToolkit _settingsToolkit;

    [ObservableProperty]
    private int _clipSkip;

    [ObservableProperty]
    private string _model;

    [ObservableProperty]
    private string _sampler;

    [ObservableProperty]
    private int _width;

    [ObservableProperty]
    private int _height;

    [ObservableProperty]
    private double _cfgScale;

    [ObservableProperty]
    private double _seed;

    [ObservableProperty]
    private int _steps;
}
