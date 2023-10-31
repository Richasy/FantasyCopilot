// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Libs.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;
using Windows.Storage;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Plugins module view model.
/// </summary>
public sealed partial class PluginsModuleViewModel : ViewModelBase, IPluginsModuleViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginsModuleViewModel"/> class.
    /// </summary>
    public PluginsModuleViewModel(
        IWorkflowService workflowService,
        ICacheToolkit cacheToolkit,
        IResourceToolkit resourceToolkit,
        IFileToolkit fileToolkit,
        IAppViewModel appViewModel,
        ILogger<PluginsModuleViewModel> logger)
    {
        _workflowService = workflowService;
        _cacheToolkit = cacheToolkit;
        _resourceToolkit = resourceToolkit;
        _fileToolkit = fileToolkit;
        _appViewModel = appViewModel;
        _logger = logger;

        Plugins = new SynchronizedObservableCollection<IPluginItemViewModel>();
        Plugins.CollectionChanged += (_, _) => CheckIsEmpty();
        CheckIsEmpty();

        AttachIsRunningToAsyncCommand(p => IsLoading = p, InitializeCommand, ReloadPluginCommand);
        AttachIsRunningToAsyncCommand(p => IsImporting = p, ImportPluginCommand);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (_isInitialized || IsLoading)
        {
            return;
        }

        await ReloadPluginAsync();
        _isInitialized = true;
    }

    [RelayCommand]
    private async Task ReloadPluginAsync()
    {
        await _workflowService.ReloadPluginsAsync();
        await RefreshPluginCacheAsync();
    }

    [RelayCommand]
    private async Task ImportPluginAsync()
    {
        var fileObj = await _fileToolkit.PickFileAsync(WorkflowConstants.PluginExtension, _appViewModel.MainWindow);
        if (fileObj is not StorageFile file)
        {
            return;
        }

        try
        {
            var config = await _cacheToolkit.GetPluginConfigFromZipAsync(file.Path);

            if (string.IsNullOrEmpty(config.Name) || string.IsNullOrEmpty(config.Description))
            {
                throw new ArgumentException(_resourceToolkit.GetLocalizedString(StringNames.MustHaveNameOrDescription));
            }

            if (string.IsNullOrEmpty(config.Id))
            {
                throw new ArgumentException(_resourceToolkit.GetLocalizedString(StringNames.MustHaveId));
            }

            if (string.IsNullOrEmpty(config.Repository))
            {
                _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.NoRepositoryWarning), InfoType.Warning);
            }

            if (config.Commands == null || config.Commands.Count == 0)
            {
                throw new ArgumentException(_resourceToolkit.GetLocalizedString(StringNames.MustHaveCommand));
            }

            foreach (var command in config.Commands)
            {
                if (string.IsNullOrEmpty(command.Name) || string.IsNullOrEmpty(command.Description))
                {
                    throw new ArgumentException(command.Name + ": " + _resourceToolkit.GetLocalizedString(StringNames.MustHaveNameOrDescription));
                }

                if (!Guid.TryParse(command.Identity, out var functionId))
                {
                    throw new ArgumentException(command.Name + ": " + _resourceToolkit.GetLocalizedString(StringNames.CommandIdMustBeGuid));
                }
            }

            await _cacheToolkit.ImportPluginConfigAsync(config, file.Path);
            await _workflowService.ReloadPluginsAsync();
            await RefreshPluginCacheAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(ImportPluginCommand));
            _appViewModel.ShowTip(ex.Message, InfoType.Error);
        }
    }

    [RelayCommand]
    private async Task DeletePluginAsync(string pluginId)
    {
        await _cacheToolkit.RemovePluginAsync(pluginId);
        await RefreshPluginCacheAsync();
    }

    private async Task RefreshPluginCacheAsync()
    {
        TryClear(Plugins);

        var plugins = await _cacheToolkit.GetPluginConfigsAsync();
        foreach (var item in plugins)
        {
            var vm = Locator.Current.GetService<IPluginItemViewModel>();
            vm.InjectData(item);
            Plugins.Add(vm);
        }
    }

    private bool CheckIsEmpty()
        => IsEmpty = Plugins.Count == 0;
}
