// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// View model of the saved sessions page.
/// </summary>
public sealed partial class SavedSessionsModuleViewModel
{
    private readonly ICacheToolkit _cacheToolkit;
    private readonly IAppViewModel _appViewModel;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    private bool _isInitialized;

    /// <inheritdoc/>
    public ObservableCollection<SessionMetadata> Sessions { get; }
}
