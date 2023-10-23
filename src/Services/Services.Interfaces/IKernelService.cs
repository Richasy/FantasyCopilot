// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Services.Interfaces;

/// <summary>
/// Chat service interface.
/// </summary>
public interface IKernelService
{
    /// <summary>
    /// Does the current user have a valid embedding model.
    /// </summary>
    bool IsChatSupport { get; }

    /// <summary>
    /// Can the kernel provide a dialog model.
    /// </summary>
    bool HasChatModel { get; }

    /// <summary>
    /// Can the kernel provide a text completion model.
    /// </summary>
    bool HasTextCompletionModel { get; }

    /// <summary>
    /// Does the current user have a valid config.
    /// </summary>
    bool IsMemorySupport { get; }

    /// <summary>
    /// AI model source for current data calculation.
    /// </summary>
    AISource CurrentAISource { get; }

    /// <summary>
    /// Reload config.
    /// </summary>
    void ReloadConfig();

    /// <summary>
    /// Get a list of available models.
    /// </summary>
    /// <param name="source">Model source.</param>
    /// <returns>Model list.</returns>
    Task<(IEnumerable<string> ChatModels, IEnumerable<string> TextCompletions, IEnumerable<string> Embeddings)> GetSupportModelsAsync(AISource source);
}
