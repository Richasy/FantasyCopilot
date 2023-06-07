// Copyright (c) Fantasy Copilot. All rights reserved.

using System.IO;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Files;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace FantasyCopilot.App.Controls;

/// <summary>
/// Storage search item.
/// </summary>
public sealed partial class StorageSearchItem : StorageSearchItemBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StorageSearchItem"/> class.
    /// </summary>
    public StorageSearchItem() => InitializeComponent();

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not FileItem data)
        {
            return;
        }

        DispatcherQueue.TryEnqueue(async () =>
        {
            var keyword = Locator.Current.GetService<IStoragePageViewModel>().GetKeyword();
            if (data.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
            {
                var lowerFileName = data.Name.ToLower();
                var startIndex = lowerFileName.IndexOf(keyword.ToLower());
                var endIndex = startIndex + keyword.Length;
                FileNameBlock.Inlines.Clear();
                FileNameBlock.Inlines.Add(new Run() { Text = data.Name[..startIndex] });
                FileNameBlock.Inlines.Add(new Run() { Text = data.Name.Substring(startIndex, keyword.Length), FontWeight = FontWeights.Bold, Foreground = (Brush)Application.Current.Resources["AccentTextFillColorPrimaryBrush"] });
                FileNameBlock.Inlines.Add(new Run() { Text = data.Name[endIndex..] });
            }
            else
            {
                FileNameBlock.Text = data.Name;
            }

            var isEmptySize = string.IsNullOrEmpty(data.FileSize);
            FileSizeBlock.Visibility = isEmptySize ? Visibility.Collapsed : Visibility.Visible;
            StorageItemThumbnail thumbnail = default;
            try
            {
                if (data.CreatedTime != DateTime.MinValue)
                {
                    if (Directory.Exists(data.Path) && isEmptySize)
                    {
                        OpenWithItem.Visibility = Visibility.Collapsed;
                        var folder = await StorageFolder.GetFolderFromPathAsync(data.Path);
                        thumbnail = await folder.GetThumbnailAsync(ThumbnailMode.SingleItem, 64);
                    }
                    else
                    {
                        OpenWithItem.Visibility = Visibility.Visible;
                        var file = await StorageFile.GetFileFromPathAsync(data.Path);
                        thumbnail = await file.GetThumbnailAsync(ThumbnailMode.SingleItem, 64);
                    }
                }
            }
            catch (Exception)
            {
                OpenWithItem.IsEnabled = false;
            }

            if (thumbnail != null)
            {
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(thumbnail);
                FileIcon.Source = bitmapImage;
                FileIcon.Visibility = Visibility.Visible;
                EmptyIcon.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmptyIcon.Visibility = Visibility.Visible;
                FileIcon.Visibility = Visibility.Collapsed;
            }
        });
    }

    private void OnItemClick(object sender, RoutedEventArgs e)
        => Locator.Current.GetService<IStoragePageViewModel>().OpenCommand.Execute(ViewModel);

    private void OnOpenInFileExplorerClick(object sender, RoutedEventArgs e)
        => Locator.Current.GetService<IStoragePageViewModel>().OpenInFileExplorerCommand.Execute(ViewModel);

    private void OnCopyPathClick(object sender, RoutedEventArgs e)
        => Locator.Current.GetService<IStoragePageViewModel>().CopyPathCommand.Execute(ViewModel);

    private void OnOpenWithClick(object sender, RoutedEventArgs e)
        => Locator.Current.GetService<IStoragePageViewModel>().OpenWithCommand.Execute(ViewModel);
}

/// <summary>
/// Base for <see cref="StorageSearchItem"/>.
/// </summary>
public class StorageSearchItemBase : ReactiveUserControl<FileItem>
{
}
