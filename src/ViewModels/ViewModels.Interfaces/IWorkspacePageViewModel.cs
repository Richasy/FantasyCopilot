// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.ViewModels.Interfaces;

/// <summary>
/// Interface definition for workspace page view model.
/// </summary>
public interface IWorkspacePageViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Whether to display the workflow panel.
    /// </summary>
    bool IsWorkflowShown { get; }

    /// <summary>
    /// Whether to display the Semantic Skills panel.
    /// </summary>
    bool IsSemanticSkillShown { get; }

    /// <summary>
    /// Whether to display the Image Skills panel.
    /// </summary>
    bool IsImageSkillShown { get; }

    /// <summary>
    /// Whether to display the Plugin panel.
    /// </summary>
    bool IsPluginShown { get; }

    /// <summary>
    /// The selected type.
    /// </summary>
    WorkspaceDataType SelectedType { get; set; }
}
