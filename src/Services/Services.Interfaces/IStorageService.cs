// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.App.Files;
using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.Services.Interfaces;

/// <summary>
/// Local storage service.
/// </summary>
public interface IStorageService : IConfigServiceBase
{
    /// <summary>
    /// Gets the matching entries.
    /// </summary>
    /// <param name="keyword">Keyword.</param>
    /// <returns>File list.</returns>
    Task<IEnumerable<FileItem>> SearchItemsAsync(string keyword);

    /// <summary>
    /// Gets the matching files.
    /// </summary>
    /// <param name="type">Search type.</param>
    /// <param name="query">Query text.</param>
    /// <returns>File list.</returns>
    Task<IEnumerable<FileItem>> SearchFilesAsync(FileSearchType type, string query);

    /// <summary>
    /// Gets the matching files.
    /// </summary>
    /// <param name="type">Search type.</param>
    /// <param name="query">Query text.</param>
    /// <returns>File list.</returns>
    Task<IEnumerable<FileItem>> SearchAudiosAsync(AudioSearchType type, string query);

    /// <summary>
    /// Gets the matching images.
    /// </summary>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    /// <param name="imageOrientation">Image orientation.</param>
    /// <param name="imageColorDepth">Image bit depth.</param>
    /// <returns>File list.</returns>
    Task<IEnumerable<FileItem>> SearchImagesAsync(int width, int height, ImageOrientation imageOrientation, ImageColorDepth imageColorDepth);
}
