﻿// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App.Workspace.Steps;

/// <summary>
/// Knowledge Base step.
/// </summary>
public class KnowledgeBaseStep
{
    /// <summary>
    /// Knowledge base id.
    /// </summary>
    public string KnowledgeBaseId { get; set; }

    /// <summary>
    /// The search pattern provided when importing the folder.
    /// </summary>
    public string FileSearchPattern { get; set; }
}
