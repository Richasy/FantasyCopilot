// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using FantasyCopilot.Models.App.Knowledge;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition for knowledge context view model.
/// </summary>
public interface IKnowledgeContextViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Is selected.
    /// </summary>
    bool IsSelected { get; set; }

    /// <summary>
    /// Context source.
    /// </summary>
    KnowledgeContext Context { get; }

    /// <summary>
    /// Inject data.
    /// </summary>
    /// <param name="context">Knowledge context.</param>
    /// <param name="isSelected">Is this context selected.</param>
    void InjectData(KnowledgeContext context, bool isSelected = false);
}
