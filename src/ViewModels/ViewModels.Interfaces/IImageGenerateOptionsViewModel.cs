// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using FantasyCopilot.Models.App.Image;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Image generate options view model.
/// </summary>
public interface IImageGenerateOptionsViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// CLIP skip.
    /// </summary>
    int ClipSkip { get; set; }

    /// <summary>
    /// Model.
    /// </summary>
    string Model { get; set; }

    /// <summary>
    /// Sampler.
    /// </summary>
    string Sampler { get; set; }

    /// <summary>
    /// Target image width.
    /// </summary>
    int Width { get; set; }

    /// <summary>
    /// Target image height.
    /// </summary>
    int Height { get; set; }

    /// <summary>
    /// CFG scale.
    /// </summary>
    double CfgScale { get; set; }

    /// <summary>
    /// Seed.
    /// </summary>
    double Seed { get; set; }

    /// <summary>
    /// Step count.
    /// </summary>
    int Steps { get; set; }

    /// <summary>
    /// Initialize view model.
    /// </summary>
    /// <param name="options">Generate options.</param>
    void Initialize(GenerateOptions options = default);

    /// <summary>
    /// Get image options.
    /// </summary>
    /// <returns><see cref="GenerateOptions"/>.</returns>
    GenerateOptions GetOptions();
}
