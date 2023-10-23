// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using RichasyAssistant.DI.Container;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls.Workspace;

/// <summary>
/// Plugin item.
/// </summary>
public sealed partial class PluginItem : PluginItemBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginItem"/> class.
    /// </summary>
    public PluginItem()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
        => TryLoadImage();

    private void OnLoaded(object sender, RoutedEventArgs e)
        => TryLoadImage();

    private void TryLoadImage()
    {
        if (LogoImage == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(ViewModel.Logo) && Uri.IsWellFormedUriString(ViewModel.Logo, UriKind.Absolute))
        {
            LogoImage.Source = new BitmapImage(new Uri(ViewModel.Logo));
        }
    }

    private async void OnDeleteItemClickAsync(object sender, RoutedEventArgs e)
    {
        var dialog = new DeleteDialog(StringNames.DeletePlugin, StringNames.DeletePluginDescription)
        {
            XamlRoot = XamlRoot,
        };
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            var vm = Locator.Current.GetService<IPluginsModuleViewModel>();
            vm.DeletePluginCommand.Execute(ViewModel.Id);
        }
    }

    private void OnCommandContainerTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}

/// <summary>
/// Base for <see cref="PluginItem"/>.
/// </summary>
public class PluginItemBase : ReactiveUserControl<IPluginItemViewModel>
{
}
