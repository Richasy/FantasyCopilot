// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.Constants;

/// <summary>
/// AI model source.
/// </summary>
public enum AISource
{
    /// <summary>
    /// From Azure Open AI.
    /// </summary>
    Azure,

    /// <summary>
    /// From Open AI.
    /// </summary>
    OpenAI,

    /// <summary>
    /// Custom models.
    /// </summary>
    Custom,
}
