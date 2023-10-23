// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App.Gpt;

/// <summary>
/// Session content structure.
/// </summary>
public sealed class Session
{
    /// <summary>
    /// Session configuration.
    /// </summary>
    public SessionOptions Options { get; set; }

    /// <summary>
    /// Last modified time.
    /// </summary>
    public DateTimeOffset ModifiedTime { get; set; }

    /// <summary>
    /// A collection of messages between me and AI.
    /// </summary>
    public List<Message> Messages { get; set; }

    /// <summary>
    /// Session identifier.
    /// </summary>
    public string Id { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Session session && Id == session.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
