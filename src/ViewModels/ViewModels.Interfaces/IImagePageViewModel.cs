// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition for image page view model.
/// </summary>
public interface IImagePageViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// The currently selected image generation method.
    /// </summary>
    ImageModuleType SelectionType { get; set; }

    /// <summary>
    /// Whether Text to Image is selected.
    /// </summary>
    bool IsTextToImageSelected { get; }

    /// <summary>
    /// Whether Image to Image is selected.
    /// </summary>
    bool IsGallerySelected { get; }
}
