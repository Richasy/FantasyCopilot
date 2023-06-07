// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.App.Image;

/// <summary>
/// Extra model package.
/// </summary>
public sealed class ExtraModelPackage
{
    /// <summary>
    /// List of embedded models.
    /// </summary>
    public IEnumerable<string> Embeddings { get; set; }

    /// <summary>
    /// List of lora models.
    /// </summary>
    public IEnumerable<string> Loras { get; set; }
}
