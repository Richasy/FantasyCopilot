// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App.Workspace;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition of image skills module view model.
/// </summary>
public interface IImageSkillsModuleViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Is the skill list empty.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Is list loading.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Is editing config.
    /// </summary>
    bool IsEditing { get; set; }

    /// <summary>
    /// Skill list.
    /// </summary>
    ObservableCollection<ImageSkillConfig> Skills { get; }

    /// <summary>
    /// Edit config command.
    /// </summary>
    IRelayCommand<ImageSkillConfig> EditConfigCommand { get; }

    /// <summary>
    /// Delete config command.
    /// </summary>
    IAsyncRelayCommand<string> DeleteConfigCommand { get; }

    /// <summary>
    /// Initialize the list.
    /// </summary>
    IAsyncRelayCommand InitializeCommand { get; }

    /// <summary>
    /// Import config.
    /// </summary>
    IAsyncRelayCommand ImportCommand { get; }

    /// <summary>
    /// Export config.
    /// </summary>
    IAsyncRelayCommand ExportCommand { get; }
}
