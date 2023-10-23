// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using RichasyAssistant.Models.App;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Plugin item view model.
/// </summary>
public sealed partial class PluginItemViewModel
{
    [ObservableProperty]
    private string _id;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private string _author;

    [ObservableProperty]
    private string _authorSite;

    [ObservableProperty]
    private string _logo;

    [ObservableProperty]
    private string _repository;

    [ObservableProperty]
    private int _commandCount;

    [ObservableProperty]
    private string _version;

    /// <inheritdoc/>
    public SynchronizedObservableCollection<IPluginCommandItemViewModel> Commands { get; set; }
}
