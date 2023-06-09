// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Semantic skill edit module view model.
/// </summary>
public sealed partial class SemanticSkillEditModuleViewModel : ViewModelBase, ISemanticSkillEditModuleViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticSkillEditModuleViewModel"/> class.
    /// </summary>
    public SemanticSkillEditModuleViewModel(
        ICacheToolkit cacheToolkit,
        IResourceToolkit resourceToolkit,
        IAppViewModel appViewModel)
    {
        _cacheToolkit = cacheToolkit;
        _resourceToolkit = resourceToolkit;
        _appViewModel = appViewModel;
    }

    /// <inheritdoc/>
    public void InjectData(SemanticSkillConfig config)
    {
        Options ??= Locator.Current.GetService<ISessionOptionsViewModel>();

        if (config == null)
        {
            _id = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
            Prompt = string.Empty;
            Options?.Initialize();
        }
        else
        {
            _id = config.Id;
            Name = config.Name;
            Description = config.Description;
            Prompt = config.Prompt;
            Options?.Initialize(config);
        }

        // In this task, only the completion of steps is of concern.
        // Therefore, there is no significance in providing a stream output.
        Options.UseStreamOutput = false;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(Name)
            || string.IsNullOrEmpty(Description)
            || string.IsNullOrEmpty(Prompt))
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.NeedFillRequiredFields), InfoType.Error);
            return;
        }

        var config = new SemanticSkillConfig();
        if (string.IsNullOrEmpty(_id))
        {
            _id = Guid.NewGuid().ToString("N");
        }

        config.Id = _id;
        config.Name = Name;
        config.Description = Description;
        config.Prompt = Prompt;
        var options = Options.GetOptions();
        config.Temperature = options.Temperature;
        config.MaxResponseTokens = options.MaxResponseTokens;
        config.TopP = options.TopP;
        config.FrequencyPenalty = options.FrequencyPenalty;
        config.PresencePenalty = options.PresencePenalty;

        await _cacheToolkit.AddOrUpdateSemanticSkillAsync(config);
        Locator.Current.GetService<ISemanticSkillsModuleViewModel>().IsEditing = false;
    }
}
