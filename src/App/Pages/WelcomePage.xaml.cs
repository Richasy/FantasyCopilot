// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Navigation;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class WelcomePage : PageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WelcomePage"/> class.
    /// </summary>
    public WelcomePage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <inheritdoc/>
    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        => CoreViewModel.ReloadAllServicesCommand.Execute(default);

    private void OnLoaded(object sender, RoutedEventArgs e)
        => Locator.Current.GetService<ISettingsPageViewModel>().Initialize();

    private void OnStartButtonClick(object sender, RoutedEventArgs e)
    {
        Locator.Current.GetService<ISettingsToolkit>().WriteLocalSetting(SettingNames.IsSkipWelcomeScreen, true);
        Locator.Current.GetService<IAppViewModel>().InitializeAsync();
    }

    private void OnImportButtonClick(object sender, RoutedEventArgs e)
        => Locator.Current.GetService<ISettingsPageViewModel>().ImportConfigurationCommand.Execute(default);
}
