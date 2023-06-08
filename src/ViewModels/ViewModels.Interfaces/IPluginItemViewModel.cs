// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App.Plugins;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition for plugin view model.
/// </summary>
public interface IPluginItemViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Plugin id.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Plugin name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Plugin description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Plugin author.
    /// </summary>
    string Author { get; }

    /// <summary>
    /// Plugin author website.
    /// </summary>
    string AuthorSite { get; }

    /// <summary>
    /// Plugin version.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Plugin logo.
    /// </summary>
    string Logo { get; }

    /// <summary>
    /// Plugin repository.
    /// </summary>
    string Repository { get; }

    /// <summary>
    /// Command count.
    /// </summary>
    int CommandCount { get; }

    /// <summary>
    /// Plugin commands.
    /// </summary>
    ObservableCollection<IPluginCommandItemViewModel> Commands { get; }

    /// <summary>
    /// Open the repository for the plugin.
    /// </summary>
    IAsyncRelayCommand OpenRepositoryCommand { get; }

    /// <summary>
    /// Open the author's personal site.
    /// </summary>
    IAsyncRelayCommand OpenAuthorSiteCommand { get; }

    /// <summary>
    /// Open the plugin directory.
    /// </summary>
    IAsyncRelayCommand OpenPluginFolderCommand { get; }

    /// <summary>
    /// Inject data.
    /// </summary>
    /// <param name="config"><see cref="PluginConfig"/>.</param>
    void InjectData(PluginConfig config);
}
