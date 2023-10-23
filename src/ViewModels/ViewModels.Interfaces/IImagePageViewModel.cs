// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.ViewModels.Interfaces;

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
