// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Pages;

/// <summary>
/// Voice service page.
/// </summary>
public sealed partial class VoicePage : VoicePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VoicePage"/> class.
    /// </summary>
    public VoicePage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.IsBackButtonShown = false;
        ModulePicker.SelectedIndex = ViewModel.IsTextToSpeechSelected ? 0 : 1;
        DownloadTip.Visibility = Locator.Current.GetService<IVoiceService>().NeedDependencies ? Visibility.Visible : Visibility.Collapsed;
    }

    private void OnModulePickerSelectionChanged(object sender, SelectionChangedEventArgs e)
        => ViewModel.IsTextToSpeechSelected = ModulePicker.SelectedIndex == 0;
}

/// <summary>
/// Base of <see cref="VoicePage"/>.
/// </summary>
public class VoicePageBase : PageBase<IVoicePageViewModel>
{
}
