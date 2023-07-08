// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Plugins;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition for plugin command item view model.
/// </summary>
public interface IPluginCommandItemViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Command name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Command description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Command parameters.
    /// </summary>
    SynchronizedObservableCollection<InputParameter> Parameters { get; }

    /// <summary>
    /// Whether the command has no extra parameters.
    /// </summary>
    bool NoParameter { get; }

    /// <summary>
    /// Category.
    /// </summary>
    string Category { get; }

    /// <summary>
    /// Inject command data.
    /// </summary>
    /// <param name="command">Command data.</param>
    void InjectData(PluginCommand command);
}
