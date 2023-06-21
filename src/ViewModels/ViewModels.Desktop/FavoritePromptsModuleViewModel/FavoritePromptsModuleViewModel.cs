// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;
using Windows.Storage;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Favorite prompts module view model.
/// </summary>
public sealed partial class FavoritePromptsModuleViewModel : ViewModelBase, IFavoritePromptsModuleViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FavoritePromptsModuleViewModel"/> class.
    /// </summary>
    public FavoritePromptsModuleViewModel(
        ICacheToolkit cacheToolkit,
        IFileToolkit fileToolkit,
        IResourceToolkit resourceToolkit,
        IAppViewModel appViewModel,
        ILogger<FavoritePromptsModuleViewModel> logger)
    {
        _cacheToolkit = cacheToolkit;
        _fileToolkit = fileToolkit;
        _resourceToolkit = resourceToolkit;
        _appViewModel = appViewModel;
        _logger = logger;
        Prompts = new ObservableCollection<SessionMetadata>();
        Prompts.CollectionChanged += OnPromptsCollectionChanged;
        _cacheToolkit.PromptListChanged += OnPromptListChanged;
        AttachIsRunningToAsyncCommand(p => IsLoading = p, InitializeCommand);
        CheckIsEmpty();
    }

    [RelayCommand]
    private void CreateSession(SessionMetadata metadata)
    {
        var copyName = metadata.Name;
        var copyDescription = metadata.Description;
        var copyPrompt = metadata.SystemPrompt;
        var newData = new SessionMetadata
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = copyName,
            Description = copyDescription,
            SystemPrompt = copyPrompt,
        };

        var vm = Locator.Current.GetService<ISessionViewModel>();
        vm.Initialize(default, newData);
        _appViewModel.Navigate(PageType.ChatSession, vm);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (IsLoading || _isInitialized)
        {
            return;
        }

        TryClear(Prompts);

        var prompts = await _cacheToolkit.GetCustomPromptsAsync();
        foreach (var session in prompts)
        {
            Prompts.Add(session);
        }

        _isInitialized = true;
    }

    [RelayCommand]
    private async Task ImportAsync()
    {
        var fileObj = await _fileToolkit.PickFileAsync(".json", _appViewModel.MainWindow);
        if (fileObj is not StorageFile file)
        {
            return;
        }

        try
        {
            var content = await FileIO.ReadTextAsync(file);
            var list = JsonSerializer.Deserialize<List<SessionMetadata>>(content);
            if (list.Count == 0)
            {
                return;
            }

            await _cacheToolkit.AddPromptsAsync(list);
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.ConfigImported), InfoType.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to import prompt list", ex);
        }
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        if (Prompts.Count == 0)
        {
            return;
        }

        var fileObj = await _fileToolkit.SaveFileAsync("Favorite_Prompts.json", _appViewModel.MainWindow);
        if(fileObj is not StorageFile file)
        {
            return;
        }

        var json = JsonSerializer.Serialize(Prompts.ToList());
        await FileIO.WriteTextAsync(file, json);
        _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.ConfigExported), InfoType.Success);
    }

    private void CheckIsEmpty()
        => IsEmpty = Prompts.Count == 0;

    private void OnPromptsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => CheckIsEmpty();

    private void OnPromptListChanged(object sender, EventArgs e)
    {
        _isInitialized = false;
        InitializeCommand.Execute(default);
    }
}
