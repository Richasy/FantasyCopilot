// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App;

/// <summary>
/// Workflow context.
/// </summary>
public sealed class WorkflowContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkflowContext"/> class.
    /// </summary>
    public WorkflowContext()
    {
        StepResults = new Dictionary<int, string>();
        StepParameters = new Dictionary<int, string>();
    }

    /// <summary>
    /// Step result updated event.
    /// </summary>
    public event EventHandler ResultUpdated;

    /// <summary>
    /// The result of each step.
    /// </summary>
    public Dictionary<int, string> StepResults { get; }

    /// <summary>
    /// The parameters of each step.
    /// </summary>
    public Dictionary<int, string> StepParameters { get; }

    /// <summary>
    /// Current step index.
    /// </summary>
    public int CurrentStepIndex { get; set; }

    /// <summary>
    /// Raise result updated event.
    /// </summary>
    public void RaiseResultUpdated()
        => ResultUpdated?.Invoke(this, EventArgs.Empty);
}
