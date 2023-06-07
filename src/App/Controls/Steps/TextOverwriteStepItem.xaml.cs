// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Text.Json;
using FantasyCopilot.Models.App.Workspace.Steps;

namespace FantasyCopilot.App.Controls.Steps;

/// <summary>
/// Text overwrite step item.
/// </summary>
public sealed partial class TextOverwriteStepItem : WorkflowStepControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextOverwriteStepItem"/> class.
    /// </summary>
    public TextOverwriteStepItem()
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
            var step = JsonSerializer.Deserialize<TextOverwriteStep>(ViewModel.Step.Detail);
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
        var data = new TextOverwriteStep
        {
            Text = text,
        };
        ViewModel.Step.Detail = JsonSerializer.Serialize(data);
    }
}
