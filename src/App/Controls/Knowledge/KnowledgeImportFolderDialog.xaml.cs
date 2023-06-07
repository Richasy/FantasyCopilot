// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Knowledge;
using FantasyCopilot.ViewModels.Interfaces;
using Windows.Storage;

namespace FantasyCopilot.App.Controls.Knowledge;

/// <summary>
/// Knowledge import folder dialog.
/// </summary>
public sealed partial class KnowledgeImportFolderDialog : ContentDialog
{
    private readonly string _folderPath;
    private readonly KnowledgeBase _sourceBase;

    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgeImportFolderDialog"/> class.
    /// </summary>
    public KnowledgeImportFolderDialog(KnowledgeBase @base, StorageFolder folder)
    {
        InitializeComponent();
        _folderPath = folder.Path;
        SourcePathBlock.Text = _folderPath;
        _sourceBase = @base;
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (string.IsNullOrEmpty(SearchPatternBox.Text))
        {
            SearchPatternBox.Text = "*.*";
        }

        var creation = new BaseCreation
        {
            FolderPath = _folderPath,
            DatabasePath = _sourceBase.DatabasePath,
            SearchPattern = SearchPatternBox.Text,
        };

        var knowledgePageVM = Locator.Current.GetService<IKnowledgePageViewModel>();
        knowledgePageVM.ImportFolderCommand.Execute(creation);
    }
}
