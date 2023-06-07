// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Steps;

/// <summary>
/// Input text step item.
/// </summary>
public sealed partial class InputClickStepItem : WorkflowStepControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputClickStepItem"/> class.
    /// </summary>
    public InputClickStepItem()
        => InitializeComponent();

    private void OnStartButtonClick(object sender, RoutedEventArgs e)
    {
        var runnerVM = Locator.Current.GetService<IWorkflowRunnerViewModel>();
        runnerVM.ExecuteCommand.Execute(default);
    }
}
