// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Data;
using RichasyAssistant.App.Controls.Steps;

namespace RichasyAssistant.App.Controls.Workspace;

/// <summary>
/// Workflow input item.
/// </summary>
public sealed partial class WorkflowInputItem : WorkflowStepControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkflowInputItem"/> class.
    /// </summary>
    public WorkflowInputItem()
        => InitializeComponent();

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
        => InitializePresenter();

    private void InitializePresenter()
    {
        if (ViewModel == null)
        {
            InputPresenter.Content = default;
            return;
        }

        WorkflowStepControlBase item = ViewModel.Step.Skill switch
        {
            SkillType.InputText => new InputTextStepItem(),
            SkillType.InputVoice => new InputVoiceStepItem(),
            SkillType.InputFile => new InputFileStepItem(),
            SkillType.InputClick => new InputClickStepItem(),
            _ => throw new NotImplementedException(),
        };

        BindViewModel(item);
        InputPresenter.Content = item;
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
