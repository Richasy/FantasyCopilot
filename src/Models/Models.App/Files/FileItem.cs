// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App.Files;

/// <summary>
/// File item.
/// </summary>
public sealed class FileItem
{
    /// <summary>
    /// File name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// File path.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// File create time.
    /// </summary>
    public DateTime CreatedTime { get; set; }

    /// <summary>
    /// File last modified time.
    /// </summary>
    public DateTime LastModifiedTime { get; set; }

    /// <summary>
    /// File size.
    /// </summary>
    public string FileSize { get; set; }

    /// <summary>
    /// File byte length.
    /// </summary>
    public long ByteLength { get; set; }

    /// <summary>
    /// Item extension. If its folder, will be "_".
    /// </summary>
    public string Extension { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is FileItem item && Path == item.Path;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Path);
}
