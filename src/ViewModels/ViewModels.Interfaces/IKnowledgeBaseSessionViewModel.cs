// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Models.App.Knowledge;
using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition for knowledge base session view model.
/// </summary>
public interface IKnowledgeBaseSessionViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Request scroll to bottom.
    /// </summary>
    event EventHandler RequestScrollToBottom;

    /// <summary>
    /// Session title.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Session description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Quick query.
    /// </summary>
    string QuickUserInput { get; set; }

    /// <summary>
    /// Advanced query.
    /// </summary>
    string AdvancedUserInput { get; set; }

    /// <summary>
    /// Session options view model.
    /// </summary>
    ISessionOptionsViewModel Options { get; }

    /// <summary>
    /// If there is an error.
    /// </summary>
    string ErrorText { get; }

    /// <summary>
    /// Is it a quick question.
    /// </summary>
    bool IsQuickQA { get; }

    /// <summary>
    /// Is it an advanced search.
    /// </summary>
    bool IsAdvancedSearch { get; }

    /// <summary>
    /// Knowledge search type.
    /// </summary>
    KnowledgeSearchType SearchType { get; set; }

    /// <summary>
    /// Whether it is waiting for the quick q&a response.
    /// </summary>
    bool IsQuickResponding { get; }

    /// <summary>
    /// Whether it is waiting for the advanced search response.
    /// </summary>
    bool IsAdvancedSearchResponding { get; }

    /// <summary>
    /// Whether it is waiting for the advanced answer response.
    /// </summary>
    bool IsAdvancedAnswerResponding { get; }

    /// <summary>
    /// In the quick q&a, is the chat history empty.
    /// </summary>
    bool IsChatEmpty { get; }

    /// <summary>
    /// In the advanced search, did not find the context.
    /// </summary>
    bool IsNoContext { get; }

    /// <summary>
    /// In advanced search, if there is no user input.
    /// </summary>
    bool IsNoUserInputWhenAdvancedSearch { get; }

    /// <summary>
    /// Is session setup in progress.
    /// </summary>
    bool IsInSettings { get; set; }

    /// <summary>
    /// Message list.
    /// </summary>
    SynchronizedObservableCollection<Message> Messages { get; }

    /// <summary>
    /// Contexts found in advanced search.
    /// </summary>
    SynchronizedObservableCollection<IKnowledgeContextViewModel> Contexts { get; }

    /// <summary>
    /// Q&A results for Advanced Search.
    /// </summary>
    Message AdvancedAnswerResult { get; }

    /// <summary>
    /// Save session command.
    /// </summary>
    IAsyncRelayCommand SaveCommand { get; }

    /// <summary>
    /// Send message command.
    /// </summary>
    IAsyncRelayCommand SendQueryCommand { get; }

    /// <summary>
    /// Generate advance answer.
    /// </summary>
    IAsyncRelayCommand GenerateAnswerCommand { get; }

    /// <summary>
    /// Stop waiting LLM responses.
    /// </summary>
    IRelayCommand CancelMessageCommand { get; }

    /// <summary>
    /// Init session.
    /// </summary>
    /// <param name="base">Knowledge base.</param>
    void Initialize(KnowledgeBase @base);
}
