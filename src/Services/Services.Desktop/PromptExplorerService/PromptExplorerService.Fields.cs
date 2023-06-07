// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.Generic;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using Microsoft.Extensions.Logging;

namespace FantasyCopilot.Services;

/// <summary>
/// Prompt explorer service.
/// </summary>
public sealed partial class PromptExplorerService
{
    private readonly IFileToolkit _fileToolkit;
    private readonly ILogger<PromptExplorerService> _logger;
    private readonly Dictionary<OnlinePromptSource, OnlinePromptList> _cacheList;
}
