// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Semantic skills module view model.
/// </summary>
public sealed partial class SemanticSkillsModuleViewModel
{
    private readonly ICacheToolkit _cacheToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IAppViewModel _appVM;
    private readonly ISemanticSkillEditModuleViewModel _editModuleVM;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEditing;

    private bool _isInitialized;

    /// <inheritdoc />
    public ObservableCollection<SemanticSkillConfig> Skills { get; }
}
