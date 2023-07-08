// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition of online prompts module view model.
/// </summary>
public interface IOnlinePromptsModuleViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Is list loading.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Is loading error.
    /// </summary>
    bool IsError { get; }

    /// <summary>
    /// Source cache time.
    /// </summary>
    string CacheTime { get; }

    /// <summary>
    /// Selected prompt source.
    /// </summary>
    OnlinePromptSource SelectedSource { get; set; }

    /// <summary>
    /// Prompt list.
    /// </summary>
    SynchronizedObservableCollection<OnlinePrompt> Prompts { get; }

    /// <summary>
    /// Prompt source list.
    /// </summary>
    SynchronizedObservableCollection<OnlinePromptSource> Sources { get; }

    /// <summary>
    /// Create session with current prompt.
    /// </summary>
    IRelayCommand<OnlinePrompt> CreateSessionCommand { get; }

    /// <summary>
    /// Copy prompt to clipboard.
    /// </summary>
    IRelayCommand<OnlinePrompt> CopyPromptCommand { get; }

    /// <summary>
    /// Pin this prompt into favorite list.
    /// </summary>
    IAsyncRelayCommand<OnlinePrompt> FavoriteCommand { get; }

    /// <summary>
    /// Change source.
    /// </summary>
    IAsyncRelayCommand<OnlinePromptSource> ChangeSourceCommand { get; }

    /// <summary>
    /// Refresh current source.
    /// </summary>
    IAsyncRelayCommand RefreshCommand { get; }
}
