// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Data;
using RichasyAssistant.App.Controls.Steps;

namespace RichasyAssistant.App.Controls.Workspace;

/// <summary>
/// Workflow output item.
/// </summary>
public sealed partial class WorkflowOutputItem : WorkflowStepControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkflowOutputItem"/> class.
    /// </summary>
    public WorkflowOutputItem()
        => InitializeComponent();

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
        => InitializePresenter();

    private void InitializePresenter()
    {
        if (ViewModel == null)
        {
            Presenter.Content = default;
            return;
        }

        WorkflowStepControlBase item = ViewModel.Step.Skill switch
        {
            SkillType.OutputText => new OutputTextStepItem(),
            SkillType.OutputVoice => new OutputVoiceStepItem(),
            SkillType.OutputImage => new OutputImageStepItem(),
            _ => throw new NotSupportedException(),
        };

        BindViewModel(item);
        Presenter.Content = item;
    }

    private void BindViewModel(WorkflowStepControlBase item)
    {
        var binding = new Binding
        {
            Source = ViewModel,
            Mode = BindingMode.OneWay,
        };
        item.SetBinding(ViewModelProperty, binding);
    }
}
