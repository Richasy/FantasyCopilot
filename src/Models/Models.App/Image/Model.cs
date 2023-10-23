// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json.Serialization;

namespace RichasyAssistant.Models.App.Image;

/// <summary>
/// Image model.
/// </summary>
public sealed class Model
{
    /// <summary>
    /// Model name.
    /// </summary>
    [JsonPropertyName("title")]
    public string Name { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Model model && Name == model.Name;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Name);
}
