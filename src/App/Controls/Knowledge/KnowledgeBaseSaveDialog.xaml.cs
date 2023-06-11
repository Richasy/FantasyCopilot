// Copyright (c) Fantasy Copilot. All rights reserved.

using System.IO;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Knowledge;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Windows.Storage;

namespace FantasyCopilot.App.Controls.Knowledge;

/// <summary>
/// Knowledge base save dialog.
/// </summary>
public sealed partial class KnowledgeBaseSaveDialog : ContentDialog
{
    private readonly string _folderPath;
    private readonly string _filePath;
    private readonly string _dbFilePath;
    private readonly KnowledgeBase _sourceBase;
    private readonly IResourceToolkit _resourceToolkit = Locator.Current.GetService<IResourceToolkit>();
    private readonly IFileToolkit _fileToolkit = Locator.Current.GetService<IFileToolkit>();

    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgeBaseSaveDialog"/> class.
    /// </summary>
    public KnowledgeBaseSaveDialog(StorageFolder folder)
        : this()
    {
        Title = _resourceToolkit.GetLocalizedString(StringNames.CreateFromFolder);
        _folderPath = folder.Path;
        SourcePathBlock.Text = _folderPath;
        SourcePathBlock.Visibility = Visibility.Visible;
        var defaultName = new DirectoryInfo(_folderPath).Name;
        BaseNameBox.Text = defaultName;
        SearchPatternBox.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgeBaseSaveDialog"/> class.
    /// </summary>
    public KnowledgeBaseSaveDialog(StorageFile file)
        : this()
    {
        Title = _resourceToolkit.GetLocalizedString(StringNames.CreateFromFile);
        _filePath = file.Path;
        SourcePathBlock.Text = _filePath;
        SourcePathBlock.Visibility = Visibility.Visible;
        var defaultName = Path.GetFileNameWithoutExtension(_filePath);
        BaseNameBox.Text = defaultName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgeBaseSaveDialog"/> class.
    /// </summary>
    public KnowledgeBaseSaveDialog(string dbFilePath)
        : this()
    {
        Title = _resourceToolkit.GetLocalizedString(StringNames.ImportKnowledgeBase);
        _dbFilePath = dbFilePath;
        DatabasePathBox.Text = _dbFilePath;
        SelectFileButton.IsEnabled = false;
        var defaultName = Path.GetFileNameWithoutExtension(_dbFilePath);
        BaseNameBox.Text = defaultName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgeBaseSaveDialog"/> class.
    /// </summary>
    public KnowledgeBaseSaveDialog(KnowledgeBase data)
        : this()
    {
        Title = _resourceToolkit.GetLocalizedString(StringNames.ModifyKnowledgeBase);
        _sourceBase = data;
        BaseNameBox.Text = data.Name;
        DatabasePathBox.Text = data.DatabasePath;
        DescriptionBox.Text = data.Description ?? string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgeBaseSaveDialog"/> class.
    /// </summary>
    private KnowledgeBaseSaveDialog()
        => InitializeComponent();

    private async void OnSelectFileButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var shouldSave = !string.IsNullOrEmpty(_folderPath) || !string.IsNullOrEmpty(_filePath);
        if (shouldSave)
        {
            var name = BaseNameBox.Text;
            if (string.IsNullOrEmpty(name))
            {
                name = "knowledge";
            }

            var fileObj = await _fileToolkit.SaveFileAsync($"{name}.db", MainWindow.Instance);
            if (fileObj is StorageFile file)
            {
                DatabasePathBox.Text = file.Path;
            }
        }
        else
        {
            var fileObj = await _fileToolkit.PickFileAsync(".db", MainWindow.Instance);
            if (fileObj is StorageFile file)
            {
                DatabasePathBox.Text = file.Path;
            }
        }
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        args.Cancel = true;
        var appVM = Locator.Current.GetService<IAppViewModel>();
        if (string.IsNullOrEmpty(BaseNameBox.Text) || string.IsNullOrEmpty(DatabasePathBox.Text))
        {
            appVM.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.NeedFillRequiredFields), InfoType.Warning);
            return;
        }

        var knowledgePageVM = Locator.Current.GetService<IKnowledgePageViewModel>();
        if (!string.IsNullOrEmpty(_folderPath))
        {
            if (string.IsNullOrEmpty(SearchPatternBox.Text))
            {
                SearchPatternBox.Text = "*.*";
            }

            var creation = new BaseCreation
            {
                FolderPath = _folderPath,
                Name = BaseNameBox.Text,
                DatabasePath = DatabasePathBox.Text,
                SearchPattern = SearchPatternBox.Text,
            };

            knowledgePageVM.CreateBaseCommand.Execute(creation);
        }
        else if (!string.IsNullOrEmpty(_filePath))
        {
            var creation = new BaseCreation
            {
                FilePath = _filePath,
                Name = BaseNameBox.Text,
                DatabasePath = DatabasePathBox.Text,
            };

            knowledgePageVM.CreateBaseCommand.Execute(creation);
        }
        else if (!string.IsNullOrEmpty(_dbFilePath))
        {
            var @base = new KnowledgeBase
            {
                DatabasePath = DatabasePathBox.Text,
                Name = BaseNameBox.Text,
                Id = Guid.NewGuid().ToString("N"),
            };

            knowledgePageVM.ImportBaseCommand.Execute(@base);
        }
        else if (_sourceBase != null)
        {
            _sourceBase.Name = BaseNameBox.Text;
            _sourceBase.DatabasePath = DatabasePathBox.Text;

            knowledgePageVM.UpdateBaseCommand.Execute(_sourceBase);
        }

        Hide();
    }
}
