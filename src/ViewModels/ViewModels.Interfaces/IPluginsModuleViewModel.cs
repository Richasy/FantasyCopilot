// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition for plugins module view model.
/// </summary>
public interface IPluginsModuleViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Is plugin loading.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Whether there is no plugin.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Whether the plug-in is being imported.
    /// </summary>
    bool IsImporting { get; }

    /// <summary>
    /// Plugins.
    /// </summary>
    SynchronizedObservableCollection<IPluginItemViewModel> Plugins { get; }

    /// <summary>
    /// Initialize the plugins.
    /// </summary>
    IAsyncRelayCommand InitializeCommand { get; }

    /// <summary>
    /// Import the plug-in.
    /// </summary>
    IAsyncRelayCommand ImportPluginCommand { get; }

    /// <summary>
    /// Remove the plug-in.
    /// </summary>
    IAsyncRelayCommand<string> DeletePluginCommand { get; }

    /// <summary>
    /// Reload the plugin list.
    /// </summary>
    IAsyncRelayCommand ReloadPluginCommand { get; }
}
