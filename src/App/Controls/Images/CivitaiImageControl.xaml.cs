// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Media.Imaging;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls.Images;

/// <summary>
/// Civitai image control.
/// </summary>
public sealed partial class CivitaiImageControl : CivitaiImageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CivitaiImageControl"/> class.
    /// </summary>
    public CivitaiImageControl()
        => InitializeComponent();

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not ICivitaiImageViewModel vm)
        {
            return;
        }

        var bitmap = new BitmapImage(new Uri(vm.ImagePath))
        {
            DecodePixelWidth = 380,
        };
        LoadingImage();
        ImageControl.Source = bitmap;
    }

    private void LoadingImage()
    {
        ImageContainer.Visibility = Visibility.Collapsed;
        ImageLoadingShimmer.Visibility = Visibility.Visible;
        ImageErrorIcon.Visibility = Visibility.Collapsed;
    }

    private void OnImageOpened(object sender, RoutedEventArgs e)
    {
        ImageContainer.Visibility = Visibility.Visible;
        ImageLoadingShimmer.Visibility = Visibility.Collapsed;
        ImageErrorIcon.Visibility = Visibility.Collapsed;
    }

    private void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
    {
        ImageContainer.Visibility = Visibility.Collapsed;
        ImageLoadingShimmer.Visibility = Visibility.Collapsed;
        ImageErrorIcon.Visibility = Visibility.Visible;
    }
}

/// <summary>
/// Base for <see cref="CivitaiImageControl"/>.
/// </summary>
public class CivitaiImageControlBase : ReactiveUserControl<ICivitaiImageViewModel>
{
}
