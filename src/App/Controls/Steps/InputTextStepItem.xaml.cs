// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Steps;

/// <summary>
/// Input text step item.
/// </summary>
public sealed partial class InputTextStepItem : WorkflowStepControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputTextStepItem"/> class.
    /// </summary>
    public InputTextStepItem()
        => InitializeComponent();

    private void OnInputBoxTextChanged(object sender, TextChangedEventArgs e)
        => StartButton.IsEnabled = InputBox.Text.Length > 0;

    private void OnStartButtonClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(InputBox.Text))
        {
            return;
        }

        var runnerVM = Locator.Current.GetService<IWorkflowRunnerViewModel>();
        runnerVM.ExecuteCommand.Execute(InputBox.Text);
    }
}
