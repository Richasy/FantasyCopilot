// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.Constants;

/// <summary>
/// Workflow step state.
/// </summary>
public enum WorkflowStepState
{
    /// <summary>
    /// Configuring.
    /// </summary>
    Configuring,

    /// <summary>
    /// Note started.
    /// </summary>
    NotStarted,

    /// <summary>
    /// Completed.
    /// </summary>
    Completed,

    /// <summary>
    /// Running.
    /// </summary>
    Running,

    /// <summary>
    /// Input.
    /// </summary>
    Input,

    /// <summary>
    /// Error.
    /// </summary>
    Error,

    /// <summary>
    /// Output.
    /// </summary>
    Output,
}
