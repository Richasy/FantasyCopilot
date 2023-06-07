// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.Generic;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Memory.Sqlite;

namespace FantasyCopilot.Services;

/// <summary>
/// Memory service.
/// </summary>
public sealed partial class MemoryService
{
    private readonly IKernelService _kernelService;
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly ILogger<MemoryService> _logger;

    private IKernel _kernel;
    private SqliteMemoryStore _memoryStore;
    private int _tempMemoryFileTotalCount;
    private int _tempMemoryFileImportedCount;
    private int _tempMemoryFileFailedCount;
    private List<string> _tempMemoryCollections;
}
