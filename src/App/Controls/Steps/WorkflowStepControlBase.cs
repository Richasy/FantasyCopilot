// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Steps;

/// <summary>
/// Custom step item base.
/// </summary>
public class WorkflowStepControlBase : ReactiveUserControl<IWorkflowStepViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkflowStepControlBase"/> class.
    /// </summary>
    public WorkflowStepControlBase()
        => WorkflowContext = Locator.Current.GetVariable<WorkflowContext>();

    /// <summary>
    /// Workflow context.
    /// </summary>
    internal WorkflowContext WorkflowContext { get; }
}
