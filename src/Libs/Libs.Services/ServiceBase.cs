// Copyright (c) Richasy Assistant. All rights reserved.

using NLog;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Service base.
/// </summary>
public abstract class ServiceBase
{
    /// <summary>
    /// Logger.
    /// </summary>
    protected Logger Logger { get; } = LogManager.GetCurrentClassLogger();
}
