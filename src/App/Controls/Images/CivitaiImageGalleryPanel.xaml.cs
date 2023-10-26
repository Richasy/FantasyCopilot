// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.DI.Container;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls.Images;

/// <summary>
/// Civitai image gallery panel.
/// </summary>
public sealed partial class CivitaiImageGalleryPanel : CivitaiImageGalleryPanelBase
{
    private bool _isInitializing;

    /// <summary>
    /// Initializes a new instance of the <see cref="CivitaiImageGalleryPanel"/> class.
    /// </summary>
    public CivitaiImageGalleryPanel()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<IImageGalleryModuleViewModel>();
        Loaded += OnLoadedAsync;
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
        => await InitializeAsync();

    private void OnSortSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded || _isInitializing)
        {
            return;
        }

        var selectType = (CivitaiSortType)SortComboBox.SelectedIndex;
        if (selectType == ViewModel.CurrentSortType)
        {
            return;
        }

        ImageViewer.ChangeView(default, 1d, default, true);
        ViewModel.CurrentSortType = selectType;
        ViewModel.RefreshCommand.Execute(default);
    }

    private void OnPeriodSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded || _isInitializing)
        {
            return;
        }

        var selectType = (CivitaiPeriodType)PeriodComboBox.SelectedIndex;
        if (selectType == ViewModel.CurrentPeriodType)
        {
            return;
        }

        ImageViewer.ChangeView(default, 1d, default, true);
        ViewModel.CurrentPeriodType = selectType;
        ViewModel.RefreshCommand.Execute(default);
    }

    private async Task InitializeAsync()
    {
        if (ViewModel.IsInitializing)
        {
            return;
        }

        _isInitializing = true;
        SortComboBox.SelectedIndex = (int)ViewModel.CurrentSortType;
        PeriodComboBox.SelectedIndex = (int)ViewModel.CurrentPeriodType;
        if (ViewModel.IsEmpty)
        {
            await ViewModel.RefreshCommand.ExecuteAsync(default);
        }

        _isInitializing = false;
    }

    private void OnImageViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        var bottomHeight = ImageViewer.ExtentHeight - ImageViewer.VerticalOffset - ImageViewer.ViewportHeight;
        if (!e.IsIntermediate
            || bottomHeight > 80
            || bottomHeight <= 0
            || ViewModel.IsInitializing)
        {
            return;
        }

        ViewModel.RequestCommand.Execute(default);
    }
}

/// <summary>
/// Base for <see cref="CivitaiImageGalleryPanel"/>.
/// </summary>
public class CivitaiImageGalleryPanelBase : ReactiveUserControl<IImageGalleryModuleViewModel>
{
}
