// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Prompt explorer service.
/// </summary>
public sealed partial class PromptExplorerService
{
    private readonly IFileToolkit _fileToolkit;
    private readonly Dictionary<OnlinePromptSource, OnlinePromptList> _cacheList;

    /// <summary>
    /// Instance.
    /// </summary>
    public static PromptExplorerService Instance { get; } = new PromptExplorerService();
}
