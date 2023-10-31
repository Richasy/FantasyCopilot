// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Voice;
using RichasyAssistant.Models.App.Workspace.Steps;
using RichasyAssistant.Libs.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;
using Windows.Storage;
using Windows.System;

namespace RichasyAssistant.App.Controls.Steps;

/// <summary>
/// Text to speech step.
/// </summary>
public sealed partial class TextToSpeechStepItem : WorkflowStepControlBase
{
    private readonly ITextToSpeechModuleViewModel _speechVM;
    private readonly IVoiceService _voiceService;
    private readonly IAppViewModel _coreViewModel;
    private List<VoiceMetadata> _voices;
    private bool _isInitializing;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextToSpeechStepItem"/> class.
    /// </summary>
    public TextToSpeechStepItem()
    {
        InitializeComponent();
        _speechVM = Locator.Current.GetService<ITextToSpeechModuleViewModel>();
        _coreViewModel = Locator.Current.GetService<IAppViewModel>();
        _voiceService = Locator.Current.GetService<IVoiceService>();
        Loaded += OnLoadedAsync;
        Unloaded += OnUnloadedAsync;
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (_coreViewModel.IsVoiceAvailable && IsLoaded)
        {
            CheckVoiceSelected();
            RefreshDescription();
        }
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        _speechVM.PropertyChanged += OnTranslateVMPropertyChanged;
        await InitializeAsync();
    }

    private void OnUnloadedAsync(object sender, RoutedEventArgs e)
        => _speechVM.PropertyChanged -= OnTranslateVMPropertyChanged;

    private void OnTranslateVMPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_speechVM.IsMetadataLoading) && !_speechVM.IsMetadataLoading)
        {
            RefreshDescription();
            if (ViewModel.State == WorkflowStepState.Configuring)
            {
                CheckVoiceSelected();
            }
        }
    }

    private async Task InitializeAsync()
    {
        if (!_coreViewModel.IsVoiceAvailable)
        {
            return;
        }

        await _speechVM.InitializeCommand.ExecuteAsync(default);
        _voices = (await _voiceService.GetVoicesAsync()).ToList();
        if (ViewModel.State == WorkflowStepState.Configuring)
        {
            CheckVoiceSelected();
        }
        else
        {
            RefreshDescription();
        }
    }

    private void CheckVoiceSelected()
    {
        if (!_coreViewModel.IsVoiceAvailable)
        {
            return;
        }

        _isInitializing = true;
        if (string.IsNullOrEmpty(ViewModel.Step.Detail))
        {
            LanguageComboBox.SelectedItem = _speechVM.SelectedCulture;
            SelectVoiceFromCulture(_speechVM.SelectedCulture);
            VoiceComboBox.SelectedIndex = 0;
            RefreshStepDetail();
        }
        else
        {
            var step = JsonSerializer.Deserialize<TextToSpeechStep>(ViewModel.Step.Detail);
            var sourceLan = _speechVM.SupportCultures.FirstOrDefault(p => p.Id == step.Language);
            LanguageComboBox.SelectedItem = sourceLan;
            SelectVoiceFromCulture(sourceLan);

            if (_voices != null)
            {
                var voice = _voices.FirstOrDefault(p => p.Id == step.Voice);
                VoiceComboBox.SelectedItem = voice;
            }
        }

        _isInitializing = false;
    }

    private async void OnPlayButtonClickAsync(object sender, RoutedEventArgs e)
    {
        if (WorkflowContext.StepResults.ContainsKey(ViewModel.Index))
        {
            var filePath = WorkflowContext.StepResults[ViewModel.Index];
            var resToolkit = Locator.Current.GetService<IResourceToolkit>();
            try
            {
                if (File.Exists(filePath))
                {
                    var file = await StorageFile.GetFileFromPathAsync(filePath);
                    await Launcher.LaunchFileAsync(file);
                }
                else
                {
                    _coreViewModel.ShowTip(resToolkit.GetLocalizedString(StringNames.InvalidFilePath), InfoType.Error);
                }
            }
            catch (Exception)
            {
                _coreViewModel.ShowTip(resToolkit.GetLocalizedString(StringNames.OpenFailed), InfoType.Error);
            }
        }
    }

    private void OnLanguageComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LanguageComboBox.SelectedItem is not LocaleInfo info || _isInitializing)
        {
            return;
        }

        SelectVoiceFromCulture(info);
        VoiceComboBox.SelectedIndex = 0;
        RefreshStepDetail();
    }

    private void OnVoiceComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (VoiceComboBox.SelectedItem is not VoiceMetadata || _isInitializing)
        {
            return;
        }

        RefreshStepDetail();
    }

    private void RefreshStepDetail()
    {
        if (LanguageComboBox.SelectedItem == null || VoiceComboBox.SelectedItem == null)
        {
            return;
        }

        var detail = string.IsNullOrEmpty(ViewModel.Step.Detail)
            ? new TextToSpeechStep()
            : JsonSerializer.Deserialize<TextToSpeechStep>(ViewModel.Step.Detail);
        detail.Language = ((LocaleInfo)LanguageComboBox.SelectedItem).Id;
        detail.Voice = ((VoiceMetadata)VoiceComboBox.SelectedItem).Id;

        ViewModel.Step.Detail = JsonSerializer.Serialize(detail);
    }

    private void RefreshDescription()
    {
        if (_voices != null && !string.IsNullOrEmpty(ViewModel.Step.Detail))
        {
            var config = JsonSerializer.Deserialize<TextToSpeechStep>(ViewModel.Step.Detail);
            if (string.IsNullOrEmpty(config.Voice))
            {
                StepContainer.StepDescription = Locator.Current.GetService<IResourceToolkit>().GetLocalizedString(StringNames.VoiceInvalid);
                return;
            }

            var voice = _voices.FirstOrDefault(p => p.Id == config.Voice);
            StepContainer.StepDescription = voice.Name;
        }
    }

    private void SelectVoiceFromCulture(LocaleInfo culture)
    {
        if (_voices == null)
        {
            return;
        }

        var voices = _voices.Where(p => p.Locale.Equals(culture.Id, StringComparison.OrdinalIgnoreCase)).ToList();
        VoiceComboBox.ItemsSource = voices;
    }
}
