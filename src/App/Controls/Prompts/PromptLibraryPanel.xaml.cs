// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Linq;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Prompts;

/// <summary>
/// Prompt library panel.
/// </summary>
public sealed partial class PromptLibraryPanel : PromptLibraryPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptLibraryPanel"/> class.
    /// </summary>
    public PromptLibraryPanel()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<IOnlinePromptsModuleViewModel>();
        Loaded += OnLoadedAsync;
    }

    private void OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedSource == OnlinePromptSource.None)
        {
            ViewModel.ChangeSourceCommand.Execute(ViewModel.Sources.First());
        }
    }

    private void OnSourceSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        var item = (OnlinePromptSource)SourceComboBox.SelectedItem;
        ListScrollViewer.ChangeView(default, 0, default);
        ViewModel.ChangeSourceCommand.Execute(item);
    }

    private void OnFavoriteButtonClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as OnlinePrompt;
        ViewModel.FavoriteCommand.Execute(data);
    }

    private void OnItemClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as OnlinePrompt;
        ViewModel.CreateSessionCommand.Execute(data);
    }

    private void OnCopyPromptButtonClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as OnlinePrompt;
        ViewModel.CopyPromptCommand.Execute(data);
    }
}

/// <summary>
/// Base of <see cref="PromptLibraryPanel"/>.
/// </summary>
public class PromptLibraryPanelBase : ReactiveUserControl<IOnlinePromptsModuleViewModel>
{
}
