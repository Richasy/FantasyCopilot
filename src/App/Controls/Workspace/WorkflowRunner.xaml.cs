// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.DI.Container;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls.Workspace;

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

    private void OnRestartButtonClick(object sender, RoutedEventArgs e)
    {
        var appVM = Locator.Current.GetService<IAppViewModel>();
        appVM.RestartAsAdminCommand.Execute(default);
    }
}

/// <summary>
/// Base for <see cref="WorkflowRunner"/>.
/// </summary>
public class WorkflowRunnerBase : ReactiveUserControl<IWorkflowRunnerViewModel>
{
}
