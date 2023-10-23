// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichasyAssistant.Models.App.Workspace;

namespace RichasyAssistant.ViewModels.Interfaces;

/// <summary>
/// Interface definition for image skill edit module view model.
/// </summary>
public interface IImageSkillEditModuleViewModel : INotifyPropertyChanged
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
    /// Negative prompt.
    /// </summary>
    string NegativePrompt { get; set; }

    /// <summary>
    /// Whether it is initializing.
    /// </summary>
    bool IsInitializing { get; }

    /// <summary>
    /// Whether the connection to the image service was lost.
    /// </summary>
    bool IsDisconnected { get; }

    /// <summary>
    /// Generate options.
    /// </summary>
    IImageGenerateOptionsViewModel Options { get; }

    /// <summary>
    /// Initialize view model.
    /// </summary>
    IAsyncRelayCommand InitializeCommand { get; }

    /// <summary>
    /// Save config.
    /// </summary>
    IAsyncRelayCommand SaveCommand { get; }

    /// <summary>
    /// Inject configuration data.
    /// </summary>
    /// <param name="config">Configuration.</param>
    void InjectData(ImageSkillConfig config);
}
