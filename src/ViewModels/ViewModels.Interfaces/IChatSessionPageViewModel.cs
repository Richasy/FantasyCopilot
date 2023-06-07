// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// New session page view model interface.
/// </summary>
public interface IChatSessionPageViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Current session.
    /// </summary>
    ISessionViewModel CurrentSession { get; }

    /// <summary>
    /// Is it in settings.
    /// </summary>
    bool IsInSettings { get; set; }

    /// <summary>
    /// Determine whether the current session is abandoned according to Id.
    /// </summary>
    IRelayCommand<string> AbandonSessionCommand { get; }

    /// <summary>
    /// Inject the new session view model.
    /// </summary>
    /// <param name="session">New session.</param>
    void Initialize(ISessionViewModel session = default);
}
