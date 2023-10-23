// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using RichasyAssistant.Models.App.Knowledge;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

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
