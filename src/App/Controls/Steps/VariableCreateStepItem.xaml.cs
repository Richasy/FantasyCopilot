// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Text.Json;
using FantasyCopilot.Models.App.Workspace.Steps;

namespace FantasyCopilot.App.Controls.Steps;

/// <summary>
/// Text overwrite step item.
/// </summary>
public sealed partial class VariableCreateStepItem : WorkflowStepControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VariableCreateStepItem"/> class.
    /// </summary>
    public VariableCreateStepItem()
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
            NameBox.Text = string.Empty;
            ValueBox.Text = string.Empty;
        }
        else
        {
            var step = JsonSerializer.Deserialize<VariableCreateStep>(ViewModel.Step.Detail);
            if (ViewModel.State == WorkflowStepState.Configuring)
            {
                NameBox.Text = step.Name;
                ValueBox.Text = step.Value;
            }
            else
            {
                StepContainer.StepDescription = $"{step.Name}";
            }
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => Initialize();

    private void OnNameBoxLostFocus(object sender, RoutedEventArgs e)
        => UpdateDetail();

    private void OnValueBoxLostFocus(object sender, RoutedEventArgs e)
        => UpdateDetail();

    private void UpdateDetail()
    {
        var detail = new VariableCreateStep
        {
            Name = NameBox.Text ?? string.Empty,
            Value = ValueBox.Text ?? string.Empty,
        };

        ViewModel.Step.Detail = JsonSerializer.Serialize(detail);
    }
}
