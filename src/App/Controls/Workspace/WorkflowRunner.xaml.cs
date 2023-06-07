﻿// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Workspace;

/// <summary>
/// Workflow runner.
/// </summary>
public sealed partial class WorkflowRunner : WorkflowRunnerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkflowRunner"/> class.
    /// </summary>
    public WorkflowRunner()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<IWorkflowRunnerViewModel>();
    }
}

/// <summary>
/// Base for <see cref="WorkflowRunner"/>.
/// </summary>
public class WorkflowRunnerBase : ReactiveUserControl<IWorkflowRunnerViewModel>
{
}
