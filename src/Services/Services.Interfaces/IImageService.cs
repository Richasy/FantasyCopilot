// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.App.Image;

namespace FantasyCopilot.Services.Interfaces;

/// <summary>
/// AI Image service.
/// </summary>
public interface IImageService : IConfigServiceBase
{
    /// <summary>
    /// Try to connect to the service.
    /// </summary>
    /// <returns>Connect result.</returns>
    Task<bool> PingAsync();

    /// <summary>
    /// Try loading if model is not initialized (can force refresh).
    /// </summary>
    /// <param name="force">Force refresh.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task InitializeModelsIfNotReadyAsync(bool force = false);

    /// <summary>
    /// Try loading if samplers is not initialized.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task InitializeSamplersIfNotReadyAsync();

    /// <summary>
    /// Try loading if extra models is not initialized (can force refresh).
    /// </summary>
    /// <param name="force">Force refresh.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task InitializeExtraModelsIfNotReadyAsync(bool force = false);

    /// <summary>
    /// Gets the model provided by the image service.
    /// </summary>
    /// <returns>Model list.</returns>
    Task<IEnumerable<Model>> GetModelsAsync();

    /// <summary>
    /// Gets the samplers provided by the picture service.
    /// </summary>
    /// <returns>Sampler list.</returns>
    Task<IEnumerable<Sampler>> GetSamplersAsync();

    /// <summary>
    /// Get additional models provided by the image service.
    /// </summary>
    /// <returns>Model list.</returns>
    Task<ExtraModelPackage> GetExtraModelsAsync();

    /// <summary>
    /// Generate image.
    /// </summary>
    /// <param name="prompt">Prompt.</param>
    /// <param name="negativePrompt">Negative prompt.</param>
    /// <param name="options">Options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Image data.</returns>
    Task<(byte[] ImageData, string Context)> GenerateImageAsync(string prompt, string negativePrompt, GenerateOptions options, CancellationToken cancellationToken);
}
