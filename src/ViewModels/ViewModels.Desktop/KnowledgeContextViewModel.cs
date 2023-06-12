// Copyright (c) Fantasy Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App.Knowledge;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Knowledge context view model.
/// </summary>
public sealed partial class KnowledgeContextViewModel : ViewModelBase, IKnowledgeContextViewModel
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private KnowledgeContext _context;

    /// <inheritdoc/>
    public void InjectData(KnowledgeContext context, bool isSelected = false)
    {
        Context = context;
        IsSelected = isSelected;
    }
}
