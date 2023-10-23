// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls.Steps;

/// <summary>
/// Output text panel.
/// </summary>
public sealed partial class OutputTextStepItem : WorkflowStepControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OutputTextStepItem"/> class.
    /// </summary>
    public OutputTextStepItem()
        => InitializeComponent();

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is IWorkflowStepViewModel oldVM)
        {
            oldVM.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (e.NewValue is IWorkflowStepViewModel newVM)
        {
            newVM.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.State))
        {
            if (ViewModel.State == WorkflowStepState.Output)
            {
                OutputBlock.Text = ViewModel.Step.Detail.Trim();
            }
        }
    }
}
