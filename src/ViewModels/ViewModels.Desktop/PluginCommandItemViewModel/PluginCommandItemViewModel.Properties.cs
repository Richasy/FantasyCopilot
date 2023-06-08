// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App.Plugins;
using FantasyCopilot.Toolkits.Interfaces;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Plugin command item view model.
/// </summary>
public sealed partial class PluginCommandItemViewModel
{
    private readonly IResourceToolkit _resourceToolkit;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private bool _noParameter;

    [ObservableProperty]
    private string _category;

    /// <inheritdoc/>
    public ObservableCollection<InputParameter> Parameters { get; }
}
