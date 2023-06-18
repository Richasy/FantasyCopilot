// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App.Knowledge;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition for knowledge page view model.
/// </summary>
public interface IKnowledgePageViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Whether the knowledge base list is empty.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Whether the knowledge base list is loading.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Whether the knowledge base is being connected.
    /// </summary>
    bool IsBaseConnecting { get; }

    /// <summary>
    /// Whether the knowledge base is being created.
    /// </summary>
    bool IsBaseCreating { get; }

    /// <summary>
    /// The total number of files when file importing.
    /// </summary>
    int TotalFileCount { get; }

    /// <summary>
    /// The number of files that have been imported.
    /// </summary>
    int ImportedFileCount { get; }

    /// <summary>
    /// The knowledge base that is currently being read.
    /// </summary>
    KnowledgeBase CurrentBase { get; }

    /// <summary>
    /// A list of knowledge bases.
    /// </summary>
    ObservableCollection<IKnowledgeBaseItemViewModel> Bases { get; }

    /// <summary>
    /// Initialize.
    /// </summary>
    IAsyncRelayCommand InitializeCommand { get; }

    /// <summary>
    /// Import the knowledge base.
    /// </summary>
    IAsyncRelayCommand<KnowledgeBase> ImportBaseCommand { get; }

    /// <summary>
    /// Create a new knowledge base.
    /// </summary>
    IAsyncRelayCommand<BaseCreation> CreateBaseCommand { get; }

    /// <summary>
    /// Import the folder into the repository.
    /// </summary>
    IAsyncRelayCommand<BaseCreation> ImportFolderCommand { get; }

    /// <summary>
    /// Import the file into the repository.
    /// </summary>
    IAsyncRelayCommand<BaseCreation> ImportFileCommand { get; }

    /// <summary>
    /// Remove the knowledge base.
    /// </summary>
    IAsyncRelayCommand<string> RemoveBaseCommand { get; }

    /// <summary>
    /// Update the name or path of the knowledge base.
    /// </summary>
    IAsyncRelayCommand<KnowledgeBase> UpdateBaseCommand { get; }

    /// <summary>
    /// Go to the Knowledge Base.
    /// </summary>
    IAsyncRelayCommand<KnowledgeBase> EnterBaseCommand { get; }

    /// <summary>
    /// Exit the knowledge base.
    /// </summary>
    IRelayCommand ExitBaseCommand { get; }
}
