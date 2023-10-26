// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.Workspace;

namespace RichasyAssistant.Services.Interfaces;

/// <summary>
/// The service can handle workflow, based on <see cref="IKernelService"/>.
/// </summary>
public interface IWorkflowService
{
    /// <summary>
    /// Execute workflow.
    /// </summary>
    /// <param name="input">Input text.</param>
    /// <param name="steps">Steps.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Execute result.</returns>
    Task<bool> ExecuteWorkflowAsync(string input, IEnumerable<WorkflowStep> steps, CancellationToken cancellationToken);

    /// <summary>
    /// Try to get workflow steps based on goal.
    /// </summary>
    /// <param name="goal">Task goal.</param>
    /// <returns>Steps.</returns>
    Task<IEnumerable<WorkflowStep>> GetWorkflowStepsFromGoalAsync(string goal);

    /// <summary>
    /// Reload all plugins.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task ReloadPluginsAsync();
}
