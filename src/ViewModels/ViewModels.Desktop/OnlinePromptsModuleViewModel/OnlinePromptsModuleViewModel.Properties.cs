// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

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
    public ObservableCollection<OnlinePrompt> Prompts { get; }

    /// <inheritdoc/>
    public ObservableCollection<OnlinePromptSource> Sources { get; }
}
