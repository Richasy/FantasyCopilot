// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App;

namespace RichasyAssistant.App.Controls.Steps;

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
