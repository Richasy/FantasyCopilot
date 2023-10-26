// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json;
using RichasyAssistant.Models.App.Workspace.Steps;

namespace RichasyAssistant.App.Controls.Steps;

/// <summary>
/// Text overwrite step item.
/// </summary>
public sealed partial class TextNotificationStepItem : WorkflowStepControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextNotificationStepItem"/> class.
    /// </summary>
    public TextNotificationStepItem()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
        => Initialize();

    private void Initialize()
    {
        if (string.IsNullOrEmpty(ViewModel.Step.Detail))
        {
            InputBox.Text = string.Empty;
        }
        else
        {
            var step = JsonSerializer.Deserialize<TextNotificationStep>(ViewModel.Step.Detail);
            if (ViewModel.State == WorkflowStepState.Configuring)
            {
                InputBox.Text = step.Text;
            }
            else
            {
                StepContainer.StepDescription = step.Text;
            }
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => Initialize();

    private void OnInputBoxLostFocus(object sender, RoutedEventArgs e)
    {
        var text = InputBox.Text;
        var data = new TextNotificationStep
        {
            Text = text,
        };
        ViewModel.Step.Detail = JsonSerializer.Serialize(data);
    }
}
