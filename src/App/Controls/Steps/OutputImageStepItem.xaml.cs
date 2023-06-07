// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using System.IO;
using System.Net.Http;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.System;

namespace FantasyCopilot.App.Controls.Steps;

/// <summary>
/// Display image.
/// </summary>
public sealed partial class OutputImageStepItem : WorkflowStepControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OutputImageStepItem"/> class.
    /// </summary>
    public OutputImageStepItem()
        => InitializeComponent();

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is IWorkflowStepViewModel oldVM)
        {
            oldVM.PropertyChanged -= OnViewModelPropertyChangedAsync;
        }

        if (e.NewValue is IWorkflowStepViewModel newVM)
        {
            HideError();
            LoadingRing.IsActive = false;
            ImageControl.Source = default;
            newVM.PropertyChanged += OnViewModelPropertyChangedAsync;
        }
    }

    private async void OnViewModelPropertyChangedAsync(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.State))
        {
            if (ViewModel.State == WorkflowStepState.Output)
            {
                HideError();
                var resToolkit = Locator.Current.GetService<IResourceToolkit>();
                try
                {
                    var path = ViewModel.Step.Detail;
                    if (path.StartsWith("http"))
                    {
                        var bitmapImage = new BitmapImage(new Uri(path));
                        ImageControl.Source = bitmapImage;
                        LoadingRing.IsActive = true;
                    }
                    else if (File.Exists(path))
                    {
                        var file = await StorageFile.GetFileFromPathAsync(path);
                        var bitmapImage = new BitmapImage();
                        using var stream = await file.OpenReadAsync();
                        await bitmapImage.SetSourceAsync(stream);
                        ImageControl.Source = bitmapImage;
                    }
                    else
                    {
                        ShowError(resToolkit.GetLocalizedString(StringNames.InvalidFilePath));
                    }
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }
    }

    private void ShowError(string msg)
    {
        ErrorContainer.Visibility = Visibility.Visible;
        ImageContainer.Visibility = Visibility.Collapsed;
        ErrorBlock.Text = string.IsNullOrEmpty(msg) ? "Unknown error" : msg;
    }

    private void HideError()
    {
        ErrorContainer.Visibility = Visibility.Collapsed;
        ImageContainer.Visibility = Visibility.Visible;
    }

    private void OnImageOpened(object sender, RoutedEventArgs e)
    {
        LoadingRing.IsActive = false;
        HideError();
    }

    private void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
    {
        LoadingRing.IsActive = false;
        ShowError(e.ErrorMessage);
    }

    private async void OnOpenItemClickAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.Step.Detail.StartsWith("http"))
        {
            await Launcher.LaunchUriAsync(new Uri(ViewModel.Step.Detail));
        }
        else
        {
            var file = await StorageFile.GetFileFromPathAsync(ViewModel.Step.Detail);
            await Launcher.LaunchFileAsync(file);
        }
    }

    private async void OnSaveItemClickAsync(object sender, RoutedEventArgs e)
    {
        var fileToolkit = Locator.Current.GetService<IFileToolkit>();
        var appVM = Locator.Current.GetService<IAppViewModel>();
        var resToolkit = Locator.Current.GetService<IResourceToolkit>();
        var fileObj = await fileToolkit.PickFileAsync(".png", MainWindow.Instance);
        if (fileObj is not StorageFile file)
        {
            return;
        }

        try
        {
            var path = ViewModel.Step.Detail;
            if (path.StartsWith("http"))
            {
                var httpClient = new HttpClient();
                var bytes = await httpClient.GetByteArrayAsync(path);
                await File.WriteAllBytesAsync(file.Path, bytes);
            }
            else
            {
                var fs = File.OpenRead(path);
                await fs.CopyToAsync(await file.OpenStreamForWriteAsync());
            }

            appVM.ShowTip(resToolkit.GetLocalizedString(StringNames.FileSaved), InfoType.Success);
        }
        catch (Exception ex)
        {
            appVM.ShowTip(ex.Message, InfoType.Error);
        }
    }
}
