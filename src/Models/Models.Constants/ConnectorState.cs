// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.Constants;

/// <summary>
/// Connector status.
/// </summary>
public enum ConnectorState
{
    /// <summary>
    /// Not started.
    /// </summary>
    NotStarted,

    /// <summary>
    /// Starting (thread has started, but service is still connecting).
    /// </summary>
    Launching,

    /// <summary>
    /// Already connected (service can ping).
    /// </summary>
    Connected,
}
