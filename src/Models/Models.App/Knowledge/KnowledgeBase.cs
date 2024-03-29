﻿// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App.Knowledge;

/// <summary>
/// Single knowledge base.
/// </summary>
public class KnowledgeBase
{
    /// <summary>
    /// Base id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Knowledge base name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Knowledge base description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Database file path.
    /// </summary>
    public string DatabasePath { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is KnowledgeBase @base && DatabasePath == @base.DatabasePath;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(DatabasePath);
}
