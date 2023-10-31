// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.SemanticKernel;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Chat service.
/// </summary>
public sealed partial class KernelService
{
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IKernel _kernel;

    /// <summary>
    /// Initializes a new instance of the <see cref="KernelService"/> class.
    /// </summary>
    public static KernelService Instance { get; } = new KernelService();

    /// <summary>
    /// Does the current user have a valid config.
    /// </summary>
    public bool HasValidConfig { get; set; }

    /// <summary>
    /// Does the current user have a valid embedding model.
    /// </summary>
    public bool IsChatSupport { get; set; }

    /// <summary>
    /// Can the kernel provide a dialog model.
    /// </summary>
    public bool HasChatModel { get; set; }

    /// <summary>
    /// Can the kernel provide a text completion model.
    /// </summary>
    public bool HasTextCompletionModel { get; set; }

    /// <summary>
    /// Does the current user have a valid config.
    /// </summary>
    public bool IsMemorySupport { get; set; }

    /// <summary>
    /// AI model source for current data calculation.
    /// </summary>
    public AISource CurrentAISource { get; set; }
}
