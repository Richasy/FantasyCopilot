// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Workspace.Steps;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Steps;

/// <summary>
/// Translate step item.
/// </summary>
public sealed partial class TranslateStepItem : WorkflowStepControlBase
{
    private readonly ITranslatePageViewModel _translateVM;
    private readonly IAppViewModel _coreViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateStepItem"/> class.
    /// </summary>
    public TranslateStepItem()
    {
        InitializeComponent();
        _translateVM = Locator.Current.GetService<ITranslatePageViewModel>();
        _coreViewModel = Locator.Current.GetService<IAppViewModel>();
        Loaded += OnLoadedAsync;
        Unloaded += OnUnloadedAsync;
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (_coreViewModel.IsTranslateAvailable && IsLoaded)
        {
            CheckLanguageSelected();
            RefreshDescription();
        }
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        _translateVM.PropertyChanged += OnTranslateVMPropertyChanged;
        await InitializeAsync();
    }

    private void OnUnloadedAsync(object sender, RoutedEventArgs e)
        => _translateVM.PropertyChanged -= OnTranslateVMPropertyChanged;

    private void OnTranslateVMPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_translateVM.IsInitializing) && !_translateVM.IsInitializing)
        {
            RefreshDescription();
            if (ViewModel.State == WorkflowStepState.Configuring)
            {
                CheckLanguageSelected();
            }
        }
    }

    private async Task InitializeAsync()
    {
        if (!_coreViewModel.IsTranslateAvailable)
        {
            return;
        }

        await _translateVM.InitializeCommand.ExecuteAsync(default);
        if (ViewModel.State == WorkflowStepState.Configuring)
        {
            CheckLanguageSelected();
        }
        else
        {
            RefreshDescription();
        }
    }

    private void CheckLanguageSelected()
    {
        if (!_coreViewModel.IsTranslateAvailable)
        {
            return;
        }

        if (string.IsNullOrEmpty(ViewModel.Step.Detail))
        {
            SourceLanguageComboBox.SelectedItem = _translateVM.SelectedSourceLanguage;
            TargetLanguageComboBox.SelectedItem = _translateVM.SelectedTargetLanguage;
        }
        else
        {
            var step = JsonSerializer.Deserialize<TranslateStep>(ViewModel.Step.Detail);
            var sourceLan = _translateVM.SourceLanguages.FirstOrDefault(p => p.Id == step.Source);
            var targetLan = _translateVM.TargetLanguages.FirstOrDefault(p => p.Id == step.Target);
            SourceLanguageComboBox.SelectedItem = sourceLan;
            TargetLanguageComboBox.SelectedItem = targetLan;
        }
    }

    private void OnSourceLanguageComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SourceLanguageComboBox.SelectedItem is not LocaleInfo info)
        {
            return;
        }

        RefreshStepDetail(true, info.Id);
    }

    private void OnTargetLanguageComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TargetLanguageComboBox.SelectedItem is not LocaleInfo info)
        {
            return;
        }

        RefreshStepDetail(false, info.Id);
    }

    private void RefreshStepDetail(bool isSource, string id)
    {
        var detail = string.IsNullOrEmpty(ViewModel.Step.Detail)
            ? new TranslateStep()
            : JsonSerializer.Deserialize<TranslateStep>(ViewModel.Step.Detail);
        if (isSource)
        {
            detail.Source = id;
        }
        else
        {
            detail.Target = id;
        }

        ViewModel.Step.Detail = JsonSerializer.Serialize(detail);
    }

    private void RefreshDescription()
    {
        if (_translateVM.SourceLanguages.Any() && !string.IsNullOrEmpty(ViewModel.Step.Detail))
        {
            var config = JsonSerializer.Deserialize<TranslateStep>(ViewModel.Step.Detail);
            if (string.IsNullOrEmpty(config.Target))
            {
                StepContainer.StepDescription = Locator.Current.GetService<IResourceToolkit>().GetLocalizedString(StringNames.TranslateInvalid);
                return;
            }

            var sourceLan = _translateVM.SourceLanguages.FirstOrDefault(p => p.Id == config.Source);
            var targetLan = _translateVM.TargetLanguages.FirstOrDefault(p => p.Id == config.Target);
            StepContainer.StepDescription = $"{sourceLan.Name} -> {targetLan.Name}";
        }
    }
}
