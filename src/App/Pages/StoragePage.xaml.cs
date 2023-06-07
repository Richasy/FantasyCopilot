// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Pages;

/// <summary>
/// Storage page.
/// </summary>
public sealed partial class StoragePage : StoragePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StoragePage"/> class.
    /// </summary>
    public StoragePage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.IsBackButtonShown = false;
        ModulePicker.SelectedIndex = Convert.ToInt32(ViewModel.SearchType);
        ImageOrientationComboBox.SelectedIndex = Convert.ToInt32(ViewModel.CurrentImageOrientation);
        ImageColorDepthComboBox.SelectedIndex = Convert.ToInt32(ViewModel.CurrentImageColorDepth);
        SortTypeComboBox.SelectedIndex = Convert.ToInt32(ViewModel.SortType);
        CheckItemFocus();
    }

    private void OnModulePickerSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var searchType = (StorageSearchType)ModulePicker.SelectedIndex;
        ViewModel.SearchType = searchType;
    }

    private void OnSearchBoxPreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            if (string.IsNullOrEmpty((sender as TextBox).Text))
            {
                return;
            }

            ViewModel.SearchCommand.Execute(default);
        }
    }

    private void CheckItemFocus()
    {
        switch (ViewModel.SearchType)
        {
            case StorageSearchType.Generic:
                GenericSearchBox?.Focus(FocusState.Programmatic);
                break;
            case StorageSearchType.File:
                FileSearchBox?.Focus(FocusState.Programmatic);
                break;
            case StorageSearchType.Audio:
                AudioSearchBox?.Focus(FocusState.Programmatic);
                break;
            default:
                break;
        }
    }

    private void OnImageOrientationSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ImageOrientationComboBox.SelectedIndex == -1 || ViewModel == null)
        {
            return;
        }

        var orientation = (ImageOrientation)ImageOrientationComboBox.SelectedIndex;
        ViewModel.CurrentImageOrientation = orientation;
    }

    private void OnImageColorDepthChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ImageColorDepthComboBox.SelectedIndex == -1 || ViewModel == null)
        {
            return;
        }

        var colorDepth = (ImageColorDepth)ImageColorDepthComboBox.SelectedIndex;
        ViewModel.CurrentImageColorDepth = colorDepth;
    }

    private void OnSortTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SortTypeComboBox.SelectedIndex == -1 || ViewModel == null)
        {
            return;
        }

        var sortType = (FileSortType)SortTypeComboBox.SelectedIndex;
        ViewModel.SortType = sortType;
    }
}

/// <summary>
/// Base for <see cref="StoragePage"/>.
/// </summary>
public class StoragePageBase : PageBase<IStoragePageViewModel>
{
}
