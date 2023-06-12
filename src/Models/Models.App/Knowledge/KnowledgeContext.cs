// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.App.Knowledge;

/// <summary>
/// Knowledge base context.
/// </summary>
public sealed class KnowledgeContext
{
    /// <summary>
    /// File name.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Context content.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Relevance score.
    /// </summary>
    public double Score { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is KnowledgeContext context && FileName == context.FileName;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(FileName);
}
