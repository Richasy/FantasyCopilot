// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.Services.Interfaces;

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
}
