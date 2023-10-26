// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Plugins;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.ViewModels;

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
    public SynchronizedObservableCollection<InputParameter> Parameters { get; }
}
