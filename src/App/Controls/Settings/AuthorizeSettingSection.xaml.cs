﻿// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Authorize;
using FantasyCopilot.Toolkits.Interfaces;

namespace FantasyCopilot.App.Controls.Settings;

/// <summary>
/// Authorization settings.
/// </summary>
public sealed partial class AuthorizeSettingSection : SettingSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeSettingSection"/> class.
    /// </summary>
    public AuthorizeSettingSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.LoadAuthorizedAppsCommand.Execute(default);

    private async void OnDeleteAppButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var resourceToolkit = Locator.Current.GetService<IResourceToolkit>();
        var tip = resourceToolkit.GetLocalizedString(StringNames.RemoveAuthorizeWarning);
        var dialog = new TipDialog(tip)
        {
            PrimaryButtonText = resourceToolkit.GetLocalizedString(StringNames.Confirm),
            CloseButtonText = resourceToolkit.GetLocalizedString(StringNames.Cancel),
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = XamlRoot,
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            var data = (sender as Button)?.DataContext as AuthorizedApp;
            ViewModel.RemoveAuthorizedAppCommand.Execute(data);
        }
    }
}
