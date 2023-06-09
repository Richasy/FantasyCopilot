// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Session view model interface.
/// </summary>
public interface ISessionViewModel : INotifyPropertyChanged
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
    /// Typing message.
    /// </summary>
    string UserInput { get; set; }

    /// <summary>
    /// Messages that are generated temporarily.
    /// </summary>
    string TempMessage { get; set; }

    /// <summary>
    /// Session options view model.
    /// </summary>
    ISessionOptionsViewModel Options { get; }

    /// <summary>
    /// If there is an error.
    /// </summary>
    string ErrorText { get; }

    /// <summary>
    /// Is continuous conversation.
    /// </summary>
    bool IsContinuousConversation { get; }

    /// <summary>
    /// Whether it is a single-round dialogue mode.
    /// </summary>
    bool IsSingleConversation { get; }

    /// <summary>
    /// Whether it is a context-based dialog mode.
    /// </summary>
    bool IsContextConversation { get; }

    /// <summary>
    /// Conversation type.
    /// </summary>
    ConversationType ConversationType { get; set; }

    /// <summary>
    /// Whether it is waiting for the service response.
    /// </summary>
    bool IsResponding { get; }

    /// <summary>
    /// Whether the chat content is empty.
    /// </summary>
    bool IsChatEmpty { get; }

    /// <summary>
    /// Whether the session is a session saved locally.
    /// </summary>
    bool IsLocalSession { get; }

    /// <summary>
    /// Whether to support creating a new blank session.
    /// </summary>
    bool AllowCreateNewSession { get; }

    /// <summary>
    /// Session id.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Whether the session service is connected.
    /// </summary>
    bool IsSessionServiceConnected { get; }

    /// <summary>
    /// Message list.
    /// </summary>
    ObservableCollection<Message> Messages { get; }

    /// <summary>
    /// Save session command.
    /// </summary>
    IAsyncRelayCommand SaveCommand { get; }

    /// <summary>
    /// Send message command.
    /// </summary>
    IAsyncRelayCommand SendMessageCommand { get; }

    /// <summary>
    /// Resent message command.
    /// </summary>
    IAsyncRelayCommand ResentMessageCommand { get; }

    /// <summary>
    /// Clear session history command.
    /// </summary>
    IRelayCommand ClearSessionCommand { get; }

    /// <summary>
    /// Delete session command.
    /// </summary>
    IAsyncRelayCommand DeleteCommand { get; }

    /// <summary>
    /// Command to remove the session and its callback.
    /// </summary>
    IRelayCommand ConnectToSessionServiceCommand { get; }

    /// <summary>
    /// Command to remove the session and its callback.
    /// </summary>
    IRelayCommand DisconnectFromSessionServiceCommand { get; }

    /// <summary>
    /// Stop waiting LLM responses.
    /// </summary>
    IRelayCommand CancelMessageCommand { get; }

    /// <summary>
    /// Remove message.
    /// </summary>
    IRelayCommand<Message> RemoveMessageCommand { get; }

    /// <summary>
    /// Init session.
    /// </summary>
    /// <param name="session">Session detail.</param>
    /// <param name="metadata">Session meta data.</param>
    void Initialize(Session session = default, SessionMetadata metadata = default);

    /// <summary>
    /// Use new metadata.
    /// </summary>
    /// <param name="metadata">Metadata.</param>
    void UseNewMetadata(SessionMetadata metadata);

    /// <summary>
    /// Get original metadata.
    /// </summary>
    /// <returns>Session metadata.</returns>
    SessionMetadata GetMetadata();
}
