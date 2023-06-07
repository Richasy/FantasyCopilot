// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Prompts;

/// <summary>
/// Favorite prompts panel.
/// </summary>
public sealed partial class FavoritePromptsPanel : FavoritePromptsPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FavoritePromptsPanel"/> class.
    /// </summary>
    public FavoritePromptsPanel()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<IFavoritePromptsModuleViewModel>();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(default);

    private void OnItemClick(object sender, RoutedEventArgs e)
    {
        var context = (sender as FrameworkElement).DataContext as SessionMetadata;
        ViewModel.CreateSessionCommand.Execute(context);
    }

    private async void OnModifyButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var context = (sender as FrameworkElement).DataContext as SessionMetadata;
        var dialog = new PromptSaveDialog(context)
        {
            XamlRoot = XamlRoot,
        };
        await dialog.ShowAsync();
    }

    private async void OnDeleteButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var context = (sender as FrameworkElement).DataContext as SessionMetadata;
        var dialog = new DeleteDialog(StringNames.DeletePrompt, StringNames.DeletePromptTip)
        {
            XamlRoot = XamlRoot,
        };
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            var cacheToolkit = Locator.Current.GetService<ICacheToolkit>();
            await cacheToolkit.DeletePromptAsync(context.Id);
        }
    }

    private async void OnAddButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var dialog = new PromptSaveDialog()
        {
            XamlRoot = XamlRoot,
        };
        await dialog.ShowAsync();
    }
}

/// <summary>
/// Base of <see cref="FavoritePromptsPanel"/>.
/// </summary>
public class FavoritePromptsPanelBase : ReactiveUserControl<IFavoritePromptsModuleViewModel>
{
}
