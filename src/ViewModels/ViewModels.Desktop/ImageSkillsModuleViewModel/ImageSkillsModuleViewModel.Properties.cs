// Copyright (c) Fantasy Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Image skills module view model.
/// </summary>
public sealed partial class ImageSkillsModuleViewModel
{
    private readonly ICacheToolkit _cacheToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IAppViewModel _appVM;
    private readonly IImageSkillEditModuleViewModel _editModuleVM;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEditing;

    private bool _isInitialized;

    /// <inheritdoc />
    public SynchronizedObservableCollection<ImageSkillConfig> Skills { get; }
}
