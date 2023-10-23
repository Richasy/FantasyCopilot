// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.Services;

/// <summary>
/// Chat service.
/// </summary>
public sealed partial class KernelService
{
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IKernel _kernel;
    private readonly ILogger<KernelService> _logger;

    /// <inheritdoc/>
    public bool IsChatSupport { get; set; }

    /// <inheritdoc/>
    public bool HasChatModel { get; set; }

    /// <inheritdoc/>
    public bool HasTextCompletionModel { get; set; }

    /// <inheritdoc/>
    public bool IsMemorySupport { get; set; }

    /// <inheritdoc/>
    public AISource CurrentAISource { get; set; }
}
