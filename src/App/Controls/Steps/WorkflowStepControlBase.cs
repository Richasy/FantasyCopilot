// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls.Steps;

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
