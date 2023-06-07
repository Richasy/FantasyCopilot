// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Windows.Storage;

namespace FantasyCopilot.App.Controls;

/// <summary>
/// Text to speech panel.
/// </summary>
public sealed partial class TextToSpeechPanel : TextToSpeechPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextToSpeechPanel"/> class.
    /// </summary>
    public TextToSpeechPanel()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<ITextToSpeechModuleViewModel>();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var appVM = Locator.Current.GetService<IAppViewModel>();
        if (appVM.IsVoiceAvailable)
        {
            ViewModel.InitializeCommand.Execute(default);
            InputBox.Focus(FocusState.Programmatic);
        }
    }

    private async void OnSaveButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var fileToolkit = Locator.Current.GetService<IFileToolkit>();
        var fileName = $"{DateTime.Now:yyyy-mm-dd_HH_mm_ss}.wav";
        var file = await fileToolkit.SaveFileAsync(fileName, MainWindow.Instance);
        if (file is StorageFile sf)
        {
            await ViewModel.SaveCommand.ExecuteAsync(sf.Path);
        }
    }
}

/// <summary>
/// Base of <see cref="TextToSpeechPanel"/>.
/// </summary>
public class TextToSpeechPanelBase : ReactiveUserControl<ITextToSpeechModuleViewModel>
{
}
