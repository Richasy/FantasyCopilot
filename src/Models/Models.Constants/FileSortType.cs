// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.Constants;

/// <summary>
/// File sort type.
/// </summary>
public enum FileSortType
{
    /// <summary>
    /// Name (A to Z).
    /// </summary>
    NameAtoZ,

    /// <summary>
    /// Name (Z to A).
    /// </summary>
    NameZtoA,

    /// <summary>
    /// Modified time (New to old).
    /// </summary>
    ModifiedTime,

    /// <summary>
    /// File type.
    /// </summary>
    Type,

    /// <summary>
    /// Size (Large to small).
    /// </summary>
    SizeLargeToSmall,

    /// <summary>
    /// Size (Small to large).
    /// </summary>
    SizeSmallToLarge,
}
