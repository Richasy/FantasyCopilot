// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Memory.Sqlite;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Memory service.
/// </summary>
public sealed partial class MemoryService
{
    private readonly ISettingsToolkit _settingsToolkit;

    private IKernel _kernel;
    private SqliteMemoryStore _memoryStore;
    private int _tempMemoryFileTotalCount;
    private int _tempMemoryFileImportedCount;
    private int _tempMemoryFileFailedCount;
    private List<string> _tempMemoryCollections;

    /// <summary>
    /// Instance.
    /// </summary>
    public static MemoryService Instance { get; } = new MemoryService();
}
