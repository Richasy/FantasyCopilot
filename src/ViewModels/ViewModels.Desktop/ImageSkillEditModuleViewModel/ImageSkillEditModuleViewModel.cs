// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App.Workspace;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Image skill edit module view model.
/// </summary>
public sealed partial class ImageSkillEditModuleViewModel : ViewModelBase, IImageSkillEditModuleViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageSkillEditModuleViewModel"/> class.
    /// </summary>
    public ImageSkillEditModuleViewModel(
        ICacheToolkit cacheToolkit,
        IResourceToolkit resourceToolkit,
        IAppViewModel appViewModel)
    {
        _cacheToolkit = cacheToolkit;
        _resourceToolkit = resourceToolkit;
        _appViewModel = appViewModel;

        AttachIsRunningToAsyncCommand(p => IsInitializing = p, InitializeCommand);
    }

    /// <inheritdoc/>
    public void InjectData(ImageSkillConfig config)
    {
        _sourceConfig = config;

        if (config == null)
        {
            _id = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
            Prompt = string.Empty;
        }
        else
        {
            _id = config.Id;
            Name = config.Name;
            Description = config.Description;
            Prompt = config.Prompt;
        }
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        var txt2imgVM = Locator.Current.GetService<ITextToImageModuleViewModel>();
        await txt2imgVM.InitializeCommand.ExecuteAsync(default);
        IsDisconnected = txt2imgVM.IsDisconnected;
        if (!IsDisconnected)
        {
            Options = Locator.Current.GetService<IImageGenerateOptionsViewModel>();
            Options.Initialize(_sourceConfig);
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(Name)
            || string.IsNullOrEmpty(Description)
            || string.IsNullOrEmpty(Prompt)
            || string.IsNullOrEmpty(Options.Model)
            || string.IsNullOrEmpty(Options.Sampler))
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.NeedFillRequiredFields), InfoType.Error);
            return;
        }

        var config = new ImageSkillConfig();
        if (string.IsNullOrEmpty(_id))
        {
            _id = Guid.NewGuid().ToString("N");
        }

        config.Id = _id;
        config.Name = Name;
        config.Description = Description;
        config.Prompt = Prompt;
        config.NegativePrompt = NegativePrompt;
        var options = Options.GetOptions();
        config.Steps = options.Steps;
        config.Sampler = options.Sampler;
        config.Width = options.Width;
        config.Height = options.Height;
        config.Seed = options.Seed;
        config.CfgScale = options.CfgScale;
        config.ClipSkip = options.ClipSkip;
        config.Model = options.Model;

        await _cacheToolkit.AddOrUpdateImageSkillAsync(config);
        Locator.Current.GetService<IImageSkillsModuleViewModel>().IsEditing = false;
    }
}
