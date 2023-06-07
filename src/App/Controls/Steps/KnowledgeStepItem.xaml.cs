// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Linq;
using System.Text.Json;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Knowledge;
using FantasyCopilot.Models.App.Workspace.Steps;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Steps;

/// <summary>
/// Knowledge step item.
/// </summary>
public sealed partial class KnowledgeStepItem : WorkflowStepControlBase
{
    private readonly ICacheToolkit _cacheToolkit;
    private readonly IKnowledgePageViewModel _knowledgePageViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgeStepItem"/> class.
    /// </summary>
    public KnowledgeStepItem()
    {
        InitializeComponent();
        _cacheToolkit = Locator.Current.GetService<ICacheToolkit>();
        _knowledgePageViewModel = Locator.Current.GetService<IKnowledgePageViewModel>();
        Loaded += OnLoadedAsync;
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (_knowledgePageViewModel.Bases.Count > 0 && IsLoaded)
        {
            CheckKnowledgeSelected();
        }
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
        => await InitializeAsync();

    private async Task InitializeAsync()
    {
        var resourceToolkit = Locator.Current.GetService<IResourceToolkit>();
        StepContainer.StepName = ViewModel.Step.Skill switch
        {
            SkillType.GetKnowledge => resourceToolkit.GetLocalizedString(StringNames.KnowledgeBaseQA),
            SkillType.ImportFileToKnowledge => resourceToolkit.GetLocalizedString(StringNames.ImportFileToKnowledgeBase),
            SkillType.ImportFolderToKnowledge => resourceToolkit.GetLocalizedString(StringNames.ImportFolderToKnowledgeBase),
            _ => throw new NotImplementedException()
        };

        SearchPatternBox.Visibility = ViewModel.Step.Skill == SkillType.ImportFolderToKnowledge ? Visibility.Visible : Visibility.Collapsed;
        AnswerButton.Visibility = ViewModel.Step.Skill == SkillType.GetKnowledge ? Visibility.Visible : Visibility.Collapsed;

        if (ViewModel.State == WorkflowStepState.Configuring)
        {
            await _knowledgePageViewModel.InitializeCommand.ExecuteAsync(default);
            CheckKnowledgeSelected();
        }
        else
        {
            var step = JsonSerializer.Deserialize<KnowledgeBaseStep>(ViewModel.Step.Detail);
            var config = (await _cacheToolkit.GetKnowledgeBasesAsync()).FirstOrDefault(p => p.Id == step.KnowledgeBaseId);
            StepContainer.StepDescription = config?.Name ?? "Unknown";
        }
    }

    private void CheckKnowledgeSelected()
    {
        if (_knowledgePageViewModel.IsEmpty)
        {
            return;
        }

        if (string.IsNullOrEmpty(ViewModel.Step.Detail))
        {
            ExistKnowledgeBaseComboBox.SelectedIndex = 0;
        }
        else
        {
            var step = JsonSerializer.Deserialize<KnowledgeBaseStep>(ViewModel.Step.Detail);
            var config = _knowledgePageViewModel.Bases.FirstOrDefault(p => p.Id == step.KnowledgeBaseId);
            ExistKnowledgeBaseComboBox.SelectedItem = config;
            SearchPatternBox.Text = step.FileSearchPattern;
        }
    }

    private void OnKnowledgeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ExistKnowledgeBaseComboBox.SelectedItem is not KnowledgeBase config)
        {
            return;
        }

        ViewModel.Step.Detail = JsonSerializer.Serialize(
            new KnowledgeBaseStep { KnowledgeBaseId = config.Id, FileSearchPattern = SearchPatternBox.Text ?? string.Empty });
    }

    private void OnSearchPatternBoxLostFocus(object sender, RoutedEventArgs e)
    {
        var baseId = string.Empty;
        if (ExistKnowledgeBaseComboBox.SelectedItem is KnowledgeBase config)
        {
            baseId = config.Id;
        }

        ViewModel.Step.Detail = JsonSerializer.Serialize(
            new KnowledgeBaseStep { KnowledgeBaseId = baseId, FileSearchPattern = SearchPatternBox.Text ?? string.Empty });
    }
}
