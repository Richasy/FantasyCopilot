// Copyright (c) Richasy Assistant. All rights reserved.

using System.Linq;
using System.Text.Json;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App.Workspace;
using RichasyAssistant.Models.App.Workspace.Steps;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls.Steps;

/// <summary>
/// Semantic step item.
/// </summary>
public sealed partial class SemanticStepItem : WorkflowStepControlBase
{
    private readonly ISemanticSkillsModuleViewModel _semanticVM;
    private readonly ICacheToolkit _cacheToolkit;

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticStepItem"/> class.
    /// </summary>
    public SemanticStepItem()
    {
        InitializeComponent();
        _semanticVM = Locator.Current.GetService<ISemanticSkillsModuleViewModel>();
        _cacheToolkit = Locator.Current.GetService<ICacheToolkit>();
        Loaded += OnLoadedAsync;
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (_semanticVM.Skills.Count > 0 && IsLoaded)
        {
            CheckSemanticSelected();
        }
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
        => await InitializeAsync();

    private async Task InitializeAsync()
    {
        if (ViewModel.State == WorkflowStepState.Configuring)
        {
            await _semanticVM.InitializeCommand.ExecuteAsync(default);
            CheckSemanticSelected();
        }
        else
        {
            var step = JsonSerializer.Deserialize<SemanticStep>(ViewModel.Step.Detail);
            var config = await _cacheToolkit.GetSemanticSkillByIdAsync(step.Id);
            StepContainer.StepDescription = config.Name;
        }
    }

    private void CheckSemanticSelected()
    {
        if (_semanticVM.IsEmpty)
        {
            return;
        }

        if (string.IsNullOrEmpty(ViewModel.Step.Detail))
        {
            ExistSemanticSkillsComboBox.SelectedIndex = 0;
        }
        else
        {
            var step = JsonSerializer.Deserialize<SemanticStep>(ViewModel.Step.Detail);
            var config = _semanticVM.Skills.FirstOrDefault(p => p.Id == step.Id);
            ExistSemanticSkillsComboBox.SelectedItem = config;
        }
    }

    private void OnSkillsComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ExistSemanticSkillsComboBox.SelectedItem is not SemanticSkillConfig config)
        {
            return;
        }

        ViewModel.Step.Detail = JsonSerializer.Serialize(new SemanticStep { Id = config.Id });
    }
}
