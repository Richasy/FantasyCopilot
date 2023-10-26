// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichasyAssistant.Models.App.Workspace;

namespace RichasyAssistant.ViewModels.Interfaces;

/// <summary>
/// Interface definition for semantic skill edit module view model.
/// </summary>
public interface ISemanticSkillEditModuleViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Config name.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Config description.
    /// </summary>
    string Description { get; set; }

    /// <summary>
    /// Prompt.
    /// </summary>
    string Prompt { get; set; }

    /// <summary>
    /// Session options.
    /// </summary>
    ISessionOptionsViewModel Options { get; }

    /// <summary>
    /// Save config.
    /// </summary>
    IAsyncRelayCommand SaveCommand { get; }

    /// <summary>
    /// Inject configuration data.
    /// </summary>
    /// <param name="config">Configuration.</param>
    void InjectData(SemanticSkillConfig config);
}
