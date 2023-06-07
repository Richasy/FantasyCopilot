// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Text.Json;
using FantasyCopilot.Models.App.Workspace.Steps;

namespace FantasyCopilot.App.Controls.Steps;

/// <summary>
/// Text overwrite step item.
/// </summary>
public sealed partial class VariableRedirectStepItem : WorkflowStepControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRedirectStepItem"/> class.
    /// </summary>
    public VariableRedirectStepItem()
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
            SourceBox.Text = string.Empty;
            TargetBox.Text = string.Empty;
        }
        else
        {
            var step = JsonSerializer.Deserialize<VariableRedirectStep>(ViewModel.Step.Detail);
            if (ViewModel.State == WorkflowStepState.Configuring)
            {
                SourceBox.Text = step.SourceName;
                TargetBox.Text = step.TargetName;
            }
            else
            {
                StepContainer.StepDescription = $"{step.SourceName} -> {step.TargetName}";
            }
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => Initialize();

    private void OnSourceBoxLostFocus(object sender, RoutedEventArgs e)
        => UpdateDetail();

    private void OnTargetBoxLostFocus(object sender, RoutedEventArgs e)
        => UpdateDetail();

    private void UpdateDetail()
    {
        var detail = new VariableRedirectStep
        {
            SourceName = SourceBox.Text ?? string.Empty,
            TargetName = TargetBox.Text ?? string.Empty,
        };

        ViewModel.Step.Detail = JsonSerializer.Serialize(detail);
    }
}
