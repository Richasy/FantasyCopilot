// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App.Voice;

/// <summary>
/// Voice metadata.
/// </summary>
public sealed class VoiceMetadata
{
    /// <summary>
    /// Voice name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Voice id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Is it a female voice.
    /// </summary>
    public bool IsFemale { get; set; }

    /// <summary>
    /// Is it a neural voice.
    /// </summary>
    public bool IsNeural { get; set; }

    /// <summary>
    /// Voice region.
    /// </summary>
    public string Locale { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VoiceMetadata metadata && Id == metadata.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
