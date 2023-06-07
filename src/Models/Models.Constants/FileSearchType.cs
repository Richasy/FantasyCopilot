// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.Constants;

/// <summary>
/// File search type.
/// </summary>
public enum FileSearchType
{
    /// <summary>
    /// Path child lookup.
    /// </summary>
    Children,

    /// <summary>
    /// Only audio files.
    /// </summary>
    Audio,

    /// <summary>
    /// Only zip files.
    /// </summary>
    Zip,

    /// <summary>
    /// Only video files.
    /// </summary>
    Video,

    /// <summary>
    /// Only pictures.
    /// </summary>
    Picture,

    /// <summary>
    /// Only execute files.
    /// </summary>
    Exe,

    /// <summary>
    /// Only documents.
    /// </summary>
    Document,

    /// <summary>
    /// Same name but different content.
    /// </summary>
    Duplicates,
}
