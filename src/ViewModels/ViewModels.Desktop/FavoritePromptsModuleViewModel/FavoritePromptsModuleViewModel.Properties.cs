// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Gpt;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Favorite prompts module view model.
/// </summary>
public sealed partial class FavoritePromptsModuleViewModel
{
    private readonly ICacheToolkit _cacheToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IAppViewModel _appViewModel;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    private bool _isInitialized;

    /// <inheritdoc/>
    public SynchronizedObservableCollection<SessionMetadata> Prompts { get; }
}
