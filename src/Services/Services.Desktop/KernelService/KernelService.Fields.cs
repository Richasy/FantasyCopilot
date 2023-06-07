// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace FantasyCopilot.Services;

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
    public bool IsMemorySupport { get; set; }

    /// <inheritdoc/>
    public bool IsSemanticSupport { get; set; }

    /// <inheritdoc/>
    public AISource CurrentAISource { get; set; }
}
