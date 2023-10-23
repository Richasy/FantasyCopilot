// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Gpt;

namespace RichasyAssistant.ViewModels.Interfaces;

/// <summary>
/// Custom prompt page view model interface.
/// </summary>
public interface IFavoritePromptsModuleViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Is the prompt list empty.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Is list loading.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Prompts list.
    /// </summary>
    SynchronizedObservableCollection<SessionMetadata> Prompts { get; }

    /// <summary>
    /// Open a saved prompt command.
    /// </summary>
    IRelayCommand<SessionMetadata> CreateSessionCommand { get; }

    /// <summary>
    /// Initialize the list.
    /// </summary>
    IAsyncRelayCommand InitializeCommand { get; }

    /// <summary>
    /// Import config.
    /// </summary>
    IAsyncRelayCommand ImportCommand { get; }

    /// <summary>
    /// Export config.
    /// </summary>
    IAsyncRelayCommand ExportCommand { get; }
}
