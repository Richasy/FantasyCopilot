// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Data;
using RichasyAssistant.App.Controls.Steps;

namespace RichasyAssistant.App.Controls.Workspace;

/// <summary>
/// Workflow step item.
/// </summary>
public sealed partial class WorkflowStepItem : WorkflowStepControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkflowStepItem"/> class.
    /// </summary>
    public WorkflowStepItem()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (IsLoaded)
        {
            InitializePresenter();
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => InitializePresenter();

    private void InitializePresenter()
    {
        var content = ViewModel.Step.Skill switch
        {
            SkillType.Semantic => (WorkflowStepControlBase)new SemanticStepItem(),
            SkillType.Translate => new TranslateStepItem(),
            SkillType.TextOverwrite => new TextOverwriteStepItem(),
            SkillType.TextToSpeech => new TextToSpeechStepItem(),
            SkillType.TextToImage => new ImageStepItem(),
            SkillType.PluginCommand => new PluginStepItem(),
            SkillType.GetKnowledge => new KnowledgeStepItem(),
            SkillType.ImportFileToKnowledge => new KnowledgeStepItem(),
            SkillType.ImportFolderToKnowledge => new KnowledgeStepItem(),
            SkillType.VariableRedirect => new VariableRedirectStepItem(),
            SkillType.VariableCreate => new VariableCreateStepItem(),
            SkillType.TextNotification => new TextNotificationStepItem(),
            _ => throw new NotImplementedException(),
        };

        BindViewModel(content);
        StepPresenter.Content = content;
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
