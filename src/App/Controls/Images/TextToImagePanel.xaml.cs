// Copyright (c) Richasy Assistant. All rights reserved.

using System.IO;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;
using Windows.ApplicationModel.DataTransfer;

namespace RichasyAssistant.App.Controls.Images;

/// <summary>
/// Text to image panel.
/// </summary>
public sealed partial class TextToImagePanel : TextToImagePanelBase
{
    private readonly IAppViewModel _appViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextToImagePanel"/> class.
    /// </summary>
    public TextToImagePanel()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<ITextToImageModuleViewModel>();
        _appViewModel = Locator.Current.GetService<IAppViewModel>();
        Loaded += OnLoadedAsync;
        Unloaded += OnUnloaded;
    }

    private void SetClipboardContent(string text)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(text);
        Clipboard.SetContent(dataPackage);
        var resourceToolkit = Locator.Current.GetService<IResourceToolkit>();
        _appViewModel.ShowTip(resourceToolkit.GetLocalizedString(StringNames.Copied));
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _appViewModel.BackRequest -= OnBackRequest;
        ViewModel.ImageGenerated -= OnImageGeneratedAsync;
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        ViewModel.InitializeCommand.Execute(default);
        ViewModel.ImageGenerated += OnImageGeneratedAsync;
        _appViewModel.BackRequest += OnBackRequest;
        _appViewModel.IsBackButtonShown = ViewModel.IsInSettings;

        if (!string.IsNullOrEmpty(ViewModel.ImageMetadata))
        {
            await RenderImageAsync(ViewModel.GetTempImageData());
        }
    }

    private void OnOptionsButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.IsInSettings = true;

    private void OnMetadataButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);

    private void OnBackRequest(object sender, EventArgs e)
        => ViewModel.IsInSettings = false;

    private async void OnImageGeneratedAsync(object sender, byte[] e)
        => await RenderImageAsync(e);

    private void OnEmbeddingItemClick(object sender, RoutedEventArgs e)
    {
        var text = ((Button)sender).DataContext.ToString();
        SetClipboardContent(text + ",");
    }

    private void OnLoraItemClick(object sender, RoutedEventArgs e)
    {
        var name = ((Button)sender).DataContext.ToString();
        var text = $"<lora:{name}:1>,";
        SetClipboardContent(text);
    }

    private async Task RenderImageAsync(byte[] data)
    {
        using var ms = new MemoryStream(data);
        var image = new BitmapImage();
        await image.SetSourceAsync(ms.AsRandomAccessStream()).AsTask();
        ContentImage.Source = image;
    }
}

/// <summary>
/// Base for <see cref="TextToImagePanel"/>.
/// </summary>
public class TextToImagePanelBase : ReactiveUserControl<ITextToImageModuleViewModel>
{
}
