// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using FantasyCopilot.Models.App.Knowledge;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Knowledge base item view model.
/// </summary>
public interface IKnowledgeBaseItemViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Base name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Base description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Base id.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Get knowledge base data.
    /// </summary>
    /// <returns><see cref="KnowledgeBase"/>.</returns>
    KnowledgeBase GetData();

    /// <summary>
    /// Inject data.
    /// </summary>
    /// <param name="data"><see cref="KnowledgeBase"/>.</param>
    void InjectData(KnowledgeBase data);
}
