// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.App.Image;

/// <summary>
/// Image generation configuration.
/// </summary>
public class GenerateOptions
{
    /// <summary>
    /// CLIP skip.
    /// </summary>
    public int ClipSkip { get; set; }

    /// <summary>
    /// Model.
    /// </summary>
    public string Model { get; set; }

    /// <summary>
    /// Sampler.
    /// </summary>
    public string Sampler { get; set; }

    /// <summary>
    /// Target image width.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Target image height.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// CFG scale.
    /// </summary>
    public double CfgScale { get; set; }

    /// <summary>
    /// Seed.
    /// </summary>
    public long Seed { get; set; }

    /// <summary>
    /// Step count.
    /// </summary>
    public int Steps { get; set; }
}
