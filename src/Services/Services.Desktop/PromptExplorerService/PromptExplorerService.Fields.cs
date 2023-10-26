// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.Services;

/// <summary>
/// Prompt explorer service.
/// </summary>
public sealed partial class PromptExplorerService
{
    private readonly IFileToolkit _fileToolkit;
    private readonly ILogger<PromptExplorerService> _logger;
    private readonly Dictionary<OnlinePromptSource, OnlinePromptList> _cacheList;
}
