// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using RichasyAssistant.Models.App.Knowledge;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Knowledge base item view model.
/// </summary>
public sealed partial class KnowledgeBaseItemViewModel : ViewModelBase, IKnowledgeBaseItemViewModel
{
    private KnowledgeBase _data;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private string _id;

    /// <inheritdoc/>
    public KnowledgeBase GetData()
        => _data;

    /// <inheritdoc/>
    public void InjectData(KnowledgeBase data)
    {
        _data = data;
        Name = data.Name;
        Id = data.Id;
        Description = data.Description;
    }

    partial void OnNameChanged(string value)
        => _data.Name = value;

    partial void OnDescriptionChanged(string value)
        => _data.Description = value;
}
