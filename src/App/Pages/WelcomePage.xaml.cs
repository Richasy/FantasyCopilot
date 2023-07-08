// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.UI.Xaml.Navigation;

namespace FantasyCopilot.App.Pages;

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
