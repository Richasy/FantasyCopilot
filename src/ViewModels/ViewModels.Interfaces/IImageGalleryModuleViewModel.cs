// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition for image gallery module view model.
/// </summary>
public interface IImageGalleryModuleViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// The current sort by.
    /// </summary>
    CivitaiSortType CurrentSortType { get; set; }

    /// <summary>
    /// The current sort period.
    /// </summary>
    CivitaiPeriodType CurrentPeriodType { get; set; }

    /// <summary>
    /// Whether it is initializing.
    /// </summary>
    bool IsInitializing { get; }

    /// <summary>
    /// Whether an incremental load is in progress.
    /// </summary>
    bool IsIncrementalLoading { get; }

    /// <summary>
    /// Is there a next page.
    /// </summary>
    bool HasNext { get; }

    /// <summary>
    /// Is image list empty.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Images.
    /// </summary>
    ObservableCollection<ICivitaiImageViewModel> Images { get; }

    /// <summary>
    /// Refresh the current list.
    /// </summary>
    IAsyncRelayCommand RefreshCommand { get; }

    /// <summary>
    /// Request a new list of pictures.
    /// </summary>
    IAsyncRelayCommand RequestCommand { get; }
}
