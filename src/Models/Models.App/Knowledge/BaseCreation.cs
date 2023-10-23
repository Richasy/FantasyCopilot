// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App.Knowledge;

/// <summary>
/// Fields Required for Database Creation.
/// </summary>
public sealed class BaseCreation : KnowledgeBase
{
    /// <summary>
    /// File search pattern.
    /// </summary>
    public string SearchPattern { get; set; }

    /// <summary>
    /// Selected folder path.
    /// </summary>
    public string FolderPath { get; set; }

    /// <summary>
    /// Selected file path.
    /// </summary>
    public string FilePath { get; set; }
}
