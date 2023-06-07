// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.App;

namespace FantasyCopilot.App.Controls.Steps;

/// <summary>
/// Button for show text result.
/// </summary>
public sealed partial class TextResultButton : WorkflowStepControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextResultButton"/> class.
    /// </summary>
    public TextResultButton()
        => InitializeComponent();

    private void OnResultButtonClick(object sender, RoutedEventArgs e)
    {
        if (WorkflowContext.StepResults.ContainsKey(ViewModel.Index))
        {
            OutputBlock.Text = WorkflowContext.StepResults[ViewModel.Index];
        }
    }
}
