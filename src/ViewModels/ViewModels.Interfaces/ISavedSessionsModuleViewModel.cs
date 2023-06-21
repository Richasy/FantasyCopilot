// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App.Gpt;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface for the view model of the saved sessions page.
/// </summary>
public interface ISavedSessionsModuleViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Is the session list empty.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Is list loading.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Session list.
    /// </summary>
    ObservableCollection<SessionMetadata> Sessions { get; }

    /// <summary>
    /// Open a saved session command.
    /// </summary>
    IAsyncRelayCommand<SessionMetadata> OpenSessionCommand { get; }

    /// <summary>
    /// Initialize the list.
    /// </summary>
    IRelayCommand InitializeCommand { get; }

    /// <summary>
    /// Import config.
    /// </summary>
    IAsyncRelayCommand ImportCommand { get; }

    /// <summary>
    /// Export config.
    /// </summary>
    IAsyncRelayCommand ExportCommand { get; }
}
