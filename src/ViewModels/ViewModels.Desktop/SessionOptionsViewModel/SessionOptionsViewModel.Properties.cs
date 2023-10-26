// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Session options view model.
/// </summary>
public sealed partial class SessionOptionsViewModel
{
    private readonly ISettingsToolkit _settingsToolkit;

    [ObservableProperty]
    private double _temperature;

    [ObservableProperty]
    private int _maxResponseTokens;

    [ObservableProperty]
    private double _topP;

    [ObservableProperty]
    private double _frequencyPenalty;

    [ObservableProperty]
    private double _presencePenalty;

    [ObservableProperty]
    private bool _useStreamOutput;

    [ObservableProperty]
    private bool _autoRemoveEarlierMessage;

    [ObservableProperty]
    private bool _isStreamOutputEnabled;
}
