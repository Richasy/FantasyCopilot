﻿// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.DI.Container;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls;

/// <summary>
/// Speech recognition panel.
/// </summary>
public sealed partial class SpeechRecognizePanel : SpeechRecognizePanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechRecognizePanel"/> class.
    /// </summary>
    public SpeechRecognizePanel()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<ISpeechRecognizeModuleViewModel>();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var appVM = Locator.Current.GetService<IAppViewModel>();
        if (appVM.IsVoiceAvailable)
        {
            ViewModel.InitializeCommand.Execute(default);
        }
    }
}

/// <summary>
/// Base of <see cref="SpeechRecognizePanel"/>.
/// </summary>
public class SpeechRecognizePanelBase : ReactiveUserControl<ISpeechRecognizeModuleViewModel>
{
}
