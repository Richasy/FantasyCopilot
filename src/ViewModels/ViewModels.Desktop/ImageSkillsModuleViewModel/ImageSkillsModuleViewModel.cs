// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Image skills module view model.
/// </summary>
public sealed partial class ImageSkillsModuleViewModel : ViewModelBase, IImageSkillsModuleViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageSkillsModuleViewModel"/> class.
    /// </summary>
    public ImageSkillsModuleViewModel(
        ICacheToolkit cacheToolkit,
        IResourceToolkit resourceToolkit,
        IAppViewModel appViewModel,
        IImageSkillEditModuleViewModel editModuleVM)
    {
        _cacheToolkit = cacheToolkit;
        _resourceToolkit = resourceToolkit;
        _appVM = appViewModel;
        _editModuleVM = editModuleVM;
        Skills = new SynchronizedObservableCollection<ImageSkillConfig>();
        Skills.CollectionChanged += OnSkillsCollectionChanged;
        _cacheToolkit.ImageSkillListChanged += OnImageSkillListChanged;
        AttachIsRunningToAsyncCommand(p => IsLoading = p, InitializeCommand);
        CheckIsEmpty();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (IsLoading || _isInitialized)
        {
            return;
        }

        TryClear(Skills);

        var skills = await _cacheToolkit.GetImageSkillsAsync();
        foreach (var skill in skills)
        {
            Skills.Add(skill);
        }

        _isInitialized = true;
    }

    [RelayCommand]
    private void EditConfig(ImageSkillConfig config)
    {
        _editModuleVM.InjectData(config);
        IsEditing = true;
    }

    [RelayCommand]
    private async Task DeleteConfigAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return;
        }

        var source = Skills.FirstOrDefault(p => p.Id == id);
        if (source == null)
        {
            return;
        }

        await _cacheToolkit.DeleteImageSkillAsync(id);
    }

    [RelayCommand]
    private async Task ImportAsync()
    {
        var result = await _cacheToolkit.ImportImageSkillsAsync();
        if (result == null)
        {
            return;
        }
        else if (result.Value)
        {
            _appVM.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.DataImported), InfoType.Success);
        }
        else
        {
            _appVM.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.ImportDataFailed), InfoType.Error);
        }
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        if (Skills.Count == 0)
        {
            return;
        }

        var result = await _cacheToolkit.ExportImageSkillsAsync();
        if (result == null)
        {
            return;
        }
        else if (result.Value)
        {
            _appVM.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.DataExported), InfoType.Success);
        }
        else
        {
            _appVM.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.ExportDataFailed), InfoType.Error);
        }
    }

    private void CheckIsEmpty()
        => IsEmpty = Skills.Count == 0;

    private void OnImageSkillListChanged(object sender, EventArgs e)
    {
        _isInitialized = false;
        InitializeCommand.Execute(default);
    }

    private void OnSkillsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => CheckIsEmpty();

    partial void OnIsEditingChanged(bool value)
    {
        var appVM = Locator.Current.GetService<IAppViewModel>();
        appVM.IsBackButtonShown = value;
    }
}
