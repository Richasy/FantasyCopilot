// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App.Image;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition for text to image module view model.
/// </summary>
public interface ITextToImageModuleViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Occurs when image generated.
    /// </summary>
    event EventHandler<byte[]> ImageGenerated;

    /// <summary>
    /// Image prompt.
    /// </summary>
    string Prompt { get; set; }

    /// <summary>
    /// Negative prompt.
    /// </summary>
    string NegativePrompt { get; set; }

    /// <summary>
    /// Whether a picture is being generated.
    /// </summary>
    bool IsGenerating { get; }

    /// <summary>
    /// Whether the image generation failed.
    /// </summary>
    bool IsFailed { get; }

    /// <summary>
    /// Whether the connection to the service was disconnected.
    /// </summary>
    bool IsDisconnected { get; }

    /// <summary>
    /// Whether the text2image is loading.
    /// </summary>
    bool IsInitializing { get; }

    /// <summary>
    /// Image meta data.
    /// </summary>
    string ImageMetadata { get; }

    /// <summary>
    /// Is in settings.
    /// </summary>
    bool IsInSettings { get; set; }

    /// <summary>
    /// Model list.
    /// </summary>
    ObservableCollection<Model> Models { get; }

    /// <summary>
    /// Sampler list.
    /// </summary>
    ObservableCollection<Sampler> Samplers { get; }

    /// <summary>
    /// Embedding list.
    /// </summary>
    ObservableCollection<string> Embeddings { get; }

    /// <summary>
    /// Lora list.
    /// </summary>
    ObservableCollection<string> Loras { get; }

    /// <summary>
    /// Generate options.
    /// </summary>
    IImageGenerateOptionsViewModel Options { get; }

    /// <summary>
    /// Initialize view model.
    /// </summary>
    IAsyncRelayCommand InitializeCommand { get; }

    /// <summary>
    /// Generate a picture.
    /// </summary>
    IAsyncRelayCommand GenerateCommand { get; }

    /// <summary>
    /// Save picture.
    /// </summary>
    IAsyncRelayCommand SaveImageCommand { get; }

    /// <summary>
    /// Refresh model list.
    /// </summary>
    IAsyncRelayCommand RefreshModelCommand { get; }

    /// <summary>
    /// Refresh extra model list.
    /// </summary>
    IAsyncRelayCommand RefreshExtraModelCommand { get; }

    /// <summary>
    /// Open the image service in a web page.
    /// </summary>
    IAsyncRelayCommand OpenInBrowserCommand { get; }

    /// <summary>
    /// Cancel generated.
    /// </summary>
    IRelayCommand CancelCommand { get; }

    /// <summary>
    /// Inject data.
    /// </summary>
    /// <param name="prompt">Prompt.</param>
    /// <param name="negativePrompt">Negative prompt.</param>
    /// <param name="options">Generate options.</param>
    void InjectData(string prompt, string negativePrompt, GenerateOptions options);

    /// <summary>
    /// Get temporary image data.
    /// </summary>
    /// <returns>Image data.</returns>
    byte[] GetTempImageData();
}
