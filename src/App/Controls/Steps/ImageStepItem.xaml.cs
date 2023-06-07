// Copyright (c) Fantasy Copilot. All rights reserved.

using System.IO;
using System.Linq;
using System.Text.Json;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.Models.App.Workspace.Steps;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Windows.Storage;
using Windows.System;

namespace FantasyCopilot.App.Controls.Steps;

/// <summary>
/// Image step item.
/// </summary>
public sealed partial class ImageStepItem : WorkflowStepControlBase
{
    private readonly IImageSkillsModuleViewModel _imageVM;
    private readonly ICacheToolkit _cacheToolkit;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageStepItem"/> class.
    /// </summary>
    public ImageStepItem()
    {
        InitializeComponent();
        _imageVM = Locator.Current.GetService<IImageSkillsModuleViewModel>();
        _cacheToolkit = Locator.Current.GetService<ICacheToolkit>();
        Loaded += OnLoadedAsync;
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (_imageVM.Skills.Count > 0 && IsLoaded)
        {
            CheckImageSelected();
        }
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
        => await InitializeAsync();

    private async Task InitializeAsync()
    {
        if (ViewModel.State == WorkflowStepState.Configuring)
        {
            await _imageVM.InitializeCommand.ExecuteAsync(default);
            CheckImageSelected();
        }
        else
        {
            var step = JsonSerializer.Deserialize<ImageStep>(ViewModel.Step.Detail);
            var config = await _cacheToolkit.GetImageSkillByIdAsync(step.Id);
            StepContainer.StepDescription = config.Name;
        }
    }

    private void CheckImageSelected()
    {
        if (_imageVM.IsEmpty)
        {
            return;
        }

        if (string.IsNullOrEmpty(ViewModel.Step.Detail))
        {
            ExistImageSkillsComboBox.SelectedIndex = 0;
        }
        else
        {
            var step = JsonSerializer.Deserialize<ImageStep>(ViewModel.Step.Detail);
            var config = _imageVM.Skills.FirstOrDefault(p => p.Id == step.Id);
            ExistImageSkillsComboBox.SelectedItem = config;
        }
    }

    private void OnSkillsComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ExistImageSkillsComboBox.SelectedItem is not ImageSkillConfig config)
        {
            return;
        }

        ViewModel.Step.Detail = JsonSerializer.Serialize(new ImageStep { Id = config.Id });
    }

    private async void OnOpenButtonClickAsync(object sender, RoutedEventArgs e)
    {
        if (WorkflowContext.StepResults.ContainsKey(ViewModel.Index))
        {
            var filePath = WorkflowContext.StepResults[ViewModel.Index];
            var resToolkit = Locator.Current.GetService<IResourceToolkit>();
            var coreVM = Locator.Current.GetService<IAppViewModel>();
            try
            {
                if (File.Exists(filePath))
                {
                    var file = await StorageFile.GetFileFromPathAsync(filePath);
                    await Launcher.LaunchFileAsync(file);
                }
                else
                {
                    coreVM.ShowTip(resToolkit.GetLocalizedString(StringNames.InvalidFilePath), InfoType.Error);
                }
            }
            catch (Exception)
            {
                coreVM.ShowTip(resToolkit.GetLocalizedString(StringNames.OpenFailed), InfoType.Error);
            }
        }
    }
}
