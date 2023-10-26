// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Online prompts module view model.
/// </summary>
public sealed partial class OnlinePromptsModuleViewModel
{
    private readonly IPromptExplorerService _promptExplorerService;
    private readonly ICacheToolkit _cacheToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IAppViewModel _appViewModel;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isError;

    [ObservableProperty]
    private OnlinePromptSource _selectedSource;

    [ObservableProperty]
    private string _cacheTime;

    /// <inheritdoc/>
    public SynchronizedObservableCollection<OnlinePrompt> Prompts { get; }

    /// <inheritdoc/>
    public SynchronizedObservableCollection<OnlinePromptSource> Sources { get; }
}
