// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.DI.Container;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;
using Windows.Storage;

namespace RichasyAssistant.App.Controls.Knowledge;

/// <summary>
/// Knowledge list panel.
/// </summary>
public sealed partial class KnowledgeListPanel : KnowledgeListPanelBase
{
    private readonly IFileToolkit _fileToolkit;

    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgeListPanel"/> class.
    /// </summary>
    public KnowledgeListPanel()
    {
        InitializeComponent();
        _fileToolkit = Locator.Current.GetService<IFileToolkit>();
    }

    private async void OnCreateFromFileItemClickAsync(object sender, RoutedEventArgs e)
    {
        var fileObj = await _fileToolkit.PickFileAsync("*", MainWindow.Instance);
        if (fileObj is not StorageFile file)
        {
            return;
        }

        var dialog = new KnowledgeBaseSaveDialog(file)
        {
            XamlRoot = XamlRoot,
        };
        await dialog.ShowAsync();
    }

    private async void OnImportKnowledgeBaseItemClickAsync(object sender, RoutedEventArgs e)
    {
        var fileObj = await _fileToolkit.PickFileAsync(".db", MainWindow.Instance);
        if (fileObj is not StorageFile file)
        {
            return;
        }

        var dialog = new KnowledgeBaseSaveDialog(file.Path)
        {
            XamlRoot = XamlRoot,
        };
        await dialog.ShowAsync();
    }

    private async void OnCreateFromFolderItemClickAsync(SplitButton sender, SplitButtonClickEventArgs args)
    {
        var folderObj = await _fileToolkit.PickFolderAsync(MainWindow.Instance);
        if (folderObj is not StorageFolder folder)
        {
            return;
        }

        var dialog = new KnowledgeBaseSaveDialog(folder)
        {
            XamlRoot = XamlRoot,
        };
        await dialog.ShowAsync();
    }
}

/// <summary>
/// Base for <see cref="KnowledgeListPanel"/>.
/// </summary>
public class KnowledgeListPanelBase : ReactiveUserControl<IKnowledgePageViewModel>
{
}
