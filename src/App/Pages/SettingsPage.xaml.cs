// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Navigation;
using RichasyAssistant.ViewModels.Interfaces;
using Windows.System;

namespace RichasyAssistant.App.Pages;

/// <summary>
/// Settings page.
/// </summary>
public sealed partial class SettingsPage : SettingsPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPage"/> class.
    /// </summary>
    public SettingsPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.IsBackButtonShown = false;
        ViewModel.Initialize();
    }

    /// <inheritdoc/>
    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        => CoreViewModel.ReloadAllServicesCommand.Execute(default);

    private async void OnLicenseTermsClickAsync(object sender, RoutedEventArgs e)
        => await Launcher.LaunchUriAsync(new Uri("ms-settings:about"));

    private async void OnPrivacyPolicyClickAsync(object sender, RoutedEventArgs e)
        => await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-general"));

    private async void OnFeedbackClickAsync(object sender, RoutedEventArgs e)
        => await Launcher.LaunchUriAsync(new Uri("mailto:z.richasy@gmail.com"));

    private void OnImportButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.ImportConfigurationCommand.Execute(default);

    private void OnExportButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.ExportConfigurationCommand.Execute(default);
}

/// <summary>
/// Base of <see cref="SettingsPage"/>.
/// </summary>
public class SettingsPageBase : PageBase<ISettingsPageViewModel>
{
}
