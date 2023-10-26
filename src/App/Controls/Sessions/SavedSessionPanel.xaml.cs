// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.DI.Container;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls.Sessions;

/// <summary>
/// Saved session panel.
/// </summary>
public sealed partial class SavedSessionPanel : SavedSessionPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SavedSessionPanel"/> class.
    /// </summary>
    public SavedSessionPanel()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<ISavedSessionsModuleViewModel>();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(default);

    private void OnItemClick(object sender, RoutedEventArgs e)
    {
        var context = (sender as FrameworkElement).DataContext as SessionMetadata;
        ViewModel.OpenSessionCommand.Execute(context);
    }

    private async void OnModifyButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var context = (sender as FrameworkElement).DataContext as SessionMetadata;
        var dialog = new SessionSaveDialog(context)
        {
            XamlRoot = XamlRoot,
        };
        await dialog.ShowAsync();
    }

    private async void OnDeleteButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var context = (sender as FrameworkElement).DataContext as SessionMetadata;
        var dialog = new DeleteDialog(StringNames.DeleteSession, StringNames.DeleteSessionTip)
        {
            XamlRoot = XamlRoot,
        };
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            var cacheToolkit = Locator.Current.GetService<ICacheToolkit>();
            await cacheToolkit.DeleteSessionAsync(context.Id);
            Locator.Current.GetService<IChatSessionPageViewModel>().AbandonSessionCommand.Execute(context.Id);
        }
    }
}

/// <summary>
/// Base for <see cref="SavedSessionPanel"/>.
/// </summary>
public class SavedSessionPanelBase : ReactiveUserControl<ISavedSessionsModuleViewModel>
{
}
