// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Text.Json.Serialization;

namespace FantasyCopilot.Models.App.Image;

/// <summary>
/// Sampler.
/// </summary>
public sealed class Sampler
{
    /// <summary>
    /// Sampler name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Sampler sampler && Name == sampler.Name;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Name);
}
