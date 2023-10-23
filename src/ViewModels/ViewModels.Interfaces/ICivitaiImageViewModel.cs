// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichasyAssistant.Models.App.Web;

namespace RichasyAssistant.ViewModels.Interfaces;

/// <summary>
/// Interface definition for Civitai image view model.
/// </summary>
public interface ICivitaiImageViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Image path.
    /// </summary>
    string ImagePath { get; }

    /// <summary>
    /// User name.
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// Whether to include image metadata.
    /// </summary>
    bool HasMetadata { get; }

    /// <summary>
    /// Positive prompt.
    /// </summary>
    string Prompt { get; }

    /// <summary>
    /// Negative prompt.
    /// </summary>
    string NegativePrompt { get; }

    /// <summary>
    /// Sampling mode.
    /// </summary>
    string Sampler { get; }

    /// <summary>
    /// Model name.
    /// </summary>
    string Model { get; }

    /// <summary>
    /// CFG scale.
    /// </summary>
    double CfgScale { get; }

    /// <summary>
    /// Generate steps.
    /// </summary>
    int Steps { get; }

    /// <summary>
    /// Seed.
    /// </summary>
    long Seed { get; }

    /// <summary>
    /// Clip skip.
    /// </summary>
    int ClipSkip { get; }

    /// <summary>
    /// Create date string.
    /// </summary>
    string CreateTime { get; }

    /// <summary>
    /// Append the metadata of the image to the Text2Image module.
    /// </summary>
    IRelayCommand NavigateToText2ImageCommand { get; }

    /// <summary>
    /// Save the picture locally.
    /// </summary>
    IAsyncRelayCommand SaveImageCommand { get; }

    /// <summary>
    /// Open the picture in browser.
    /// </summary>
    IAsyncRelayCommand OpenInBrowserCommand { get; }

    /// <summary>
    /// Search for the specified model.
    /// </summary>
    IAsyncRelayCommand SearchModelCommand { get; }

    /// <summary>
    /// Inject image data.
    /// </summary>
    /// <param name="image">Image data.</param>
    void InjectData(CivitaiImage image);
}
