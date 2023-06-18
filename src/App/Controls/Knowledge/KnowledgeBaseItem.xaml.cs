// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Knowledge;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Windows.Storage;

namespace FantasyCopilot.App.Controls.Knowledge;

/// <summary>
/// Knowledge base item.
/// </summary>
public sealed partial class KnowledgeBaseItem : KnowledgeBaseItemBase
{
    private readonly IKnowledgePageViewModel _knowledgePageViewModel = Locator.Current.GetService<IKnowledgePageViewModel>();
    private readonly IFileToolkit _fileToolkit = Locator.Current.GetService<IFileToolkit>();

    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgeBaseItem"/> class.
    /// </summary>
    public KnowledgeBaseItem() => InitializeComponent();

    private async void OnModifyButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var context = (sender as FrameworkElement)?.DataContext as IKnowledgeBaseItemViewModel;
        var dialog = new KnowledgeBaseSaveDialog(context.GetData())
        {
            XamlRoot = XamlRoot,
        };
        await dialog.ShowAsync();
    }

    private async void OnDeleteButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var deleteDialog = new DeleteDialog(StringNames.DeleteKnowledgeBase, StringNames.DeleteKnowledgeBaseDescription)
        {
            XamlRoot = XamlRoot,
        };
        var result = await deleteDialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await _knowledgePageViewModel.RemoveBaseCommand.ExecuteAsync(ViewModel.Id);
        }
    }

    private void OnItemClick(object sender, RoutedEventArgs e)
        => _knowledgePageViewModel.EnterBaseCommand.Execute(ViewModel.GetData());

    private async void OnImportFolderItemClickAsync(object sender, RoutedEventArgs e)
    {
        var folderObj = await _fileToolkit.PickFolderAsync(MainWindow.Instance);
        if (folderObj is not StorageFolder folder)
        {
            return;
        }

        var dialog = new KnowledgeImportFolderDialog(ViewModel.GetData(), folder)
        {
            XamlRoot = XamlRoot,
        };

        await dialog.ShowAsync();
    }

    private async void OnImportFileItemClickAsync(object sender, RoutedEventArgs e)
    {
        var fileObj = await _fileToolkit.PickFileAsync("*", MainWindow.Instance);
        if (fileObj is not StorageFile file)
        {
            return;
        }

        var creation = new BaseCreation
        {
            FilePath = file.Path,
            DatabasePath = ViewModel.GetData().DatabasePath,
        };

        _knowledgePageViewModel.ImportFileCommand.Execute(creation);
    }
}

/// <summary>
/// Base for <see cref="KnowledgeBaseItem"/>.
/// </summary>
public class KnowledgeBaseItemBase : ReactiveUserControl<IKnowledgeBaseItemViewModel>
{
}
