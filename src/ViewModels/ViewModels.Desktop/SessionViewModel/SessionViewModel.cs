// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.UI.Dispatching;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Session view model.
/// </summary>
public sealed partial class SessionViewModel : ViewModelBase, ISessionViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SessionViewModel"/> class.
    /// </summary>
    public SessionViewModel(
        ICacheToolkit cacheToolkit,
        IResourceToolkit resourceToolkit,
        ISettingsToolkit settingsToolkit,
        ISessionService sessionService,
        IChatService chatService)
    {
        _cacheToolkit = cacheToolkit;
        _resourceToolkit = resourceToolkit;
        _settingsToolkit = settingsToolkit;
        _sessionService = sessionService;
        _chatService = chatService;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        ConversationType = settingsToolkit.ReadLocalSetting(SettingNames.LastConversationType, ConversationType.Continuous);
        Messages = new ObservableCollection<Message>();
        Name = _resourceToolkit.GetLocalizedString(StringNames.NewSession);
        UserInput = string.Empty;
        AttachIsRunningToAsyncCommand(
                p => IsResponding = p,
                SendMessageCommand,
                ResentMessageCommand);
        _chatService.CharacterReceived += OnChatServiceCharacterReceived;
        Messages.CollectionChanged += OnMessageCollectionChanged;
    }

    /// <inheritdoc/>
    public void Initialize(Session session = default, SessionMetadata metadata = default)
    {
        TryClear(Messages);
        UserInput = string.Empty;
        _sourceSession = session;
        UseNewMetadata(metadata);
        CheckChatEmpty();

        var options = Locator.Current.GetService<ISessionOptionsViewModel>();
        options.Initialize(session?.Options);
        Options = options;
        if (session != null && (session.Messages?.Any() ?? false))
        {
            foreach (var msg in session.Messages)
            {
                Messages.Add(msg);
            }
        }

        ConnectToSessionService();
        _sessionService.UpdateMessages(Messages.ToList());
        _sessionService.UpdateSessionOptions(options.GetOptions());
    }

    /// <inheritdoc/>
    public void UseNewMetadata(SessionMetadata metadata)
    {
        _sourceMetadata = metadata;
        AllowCreateNewSession = false;
        if (metadata != null)
        {
            Id = metadata.Id;
            IsLocalSession = _sourceSession != null;
            AllowCreateNewSession = true;
            Name = metadata.Name;
            Description = metadata.Description;
        }

        _sessionService.CreateNewChat(_sourceMetadata?.SystemPrompt ?? string.Empty);
    }

    /// <inheritdoc/>
    public SessionMetadata GetMetadata()
        => _sourceMetadata;

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (_sourceMetadata == null)
        {
            throw new InvalidOperationException("Should have metadata first.");
        }

        _sourceSession ??= new Session();
        _sourceSession.Options = Options.GetOptions();
        _sourceSession.Id = _sourceMetadata.Id;
        _sourceSession.Messages = _sessionService.GetFullMessages().ToList();

        await _cacheToolkit.SaveSessionAsync(_sourceSession);
        IsLocalSession = true;
        AllowCreateNewSession = true;
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrEmpty(UserInput))
        {
            return;
        }

        CancelMessage();
        _cancellationTokenSource = new CancellationTokenSource();

        CheckConversationType();
        ErrorText = string.Empty;
        if (!IsContinuousConversation)
        {
            TryClear(Messages);
            _sessionService.UpdateMessages(default);
        }

        var msg = UserInput;
        UserInput = string.Empty;
        await _sessionService.SendMessageAsync(msg, IsContextConversation, _cancellationTokenSource.Token);
        _cancellationTokenSource = default;
    }

    [RelayCommand]
    private async Task ResentMessageAsync()
    {
        ErrorText = string.Empty;
        CancelMessage();
        _cancellationTokenSource = new CancellationTokenSource();
        await _sessionService.SendMessageAsync(default, IsContextConversation, _cancellationTokenSource.Token);
        _cancellationTokenSource = default;
    }

    [RelayCommand]
    private void ClearSession()
    {
        TryClear(Messages);
        _sessionService.UpdateMessages(default);
        ErrorText = string.Empty;
    }

    [RelayCommand]
    private Task Delete()
        => _cacheToolkit.DeleteSessionAsync(Id);

    [RelayCommand]
    private void ConnectToSessionService()
    {
        if (IsSessionServiceConnected)
        {
            return;
        }

        _sessionService.MessageReceived += OnMessageReceived;
        _sessionService.ExceptionThrown += OnExceptionThrown;
        IsSessionServiceConnected = true;
    }

    [RelayCommand]
    private void DisconnectFromSessionService()
    {
        _sessionService.MessageReceived -= OnMessageReceived;
        _sessionService.ExceptionThrown -= OnExceptionThrown;
        IsSessionServiceConnected = false;
    }

    [RelayCommand]
    private void CancelMessage()
    {
        if (_cancellationTokenSource != null && _cancellationTokenSource.Token.CanBeCanceled)
        {
            try
            {
                _cancellationTokenSource?.Cancel();
            }
            catch (Exception)
            {
            }

            _cancellationTokenSource = default;
            _dispatcherQueue.TryEnqueue(() =>
            {
                TempMessage = string.Empty;
            });
        }
    }

    [RelayCommand]
    private void RemoveMessage(Message msg)
    {
        var index = Messages.IndexOf(msg);
        if (index == -1)
        {
            return;
        }

        _sessionService.RemoveMessage(index);
        Messages.RemoveAt(index);
    }

    private void OnExceptionThrown(object sender, Exception e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (e is TaskCanceledException || e.Message.Contains(AppConstants.MessageCancelledContent))
            {
                _sessionService.RemoveMessage(Messages.Count - 1);
                var lastMessage = Messages.Last();
                Messages.RemoveAt(Messages.Count - 1);
                if (string.IsNullOrEmpty(UserInput))
                {
                    UserInput = lastMessage.Content;
                }

                return;
            }

            TempMessage = string.Empty;
            ErrorText = e.Message;
        });
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (e.Messages != null)
            {
                foreach (var item in e.Messages)
                {
                    Messages.Add(item);
                }
            }

            TempMessage = string.Empty;
            RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
        });
    }

    private void OnChatServiceCharacterReceived(object sender, string e)
        => TempMessage = e;

    private void CheckChatEmpty()
        => IsChatEmpty = Messages.Count == 0;

    private void OnMessageCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => CheckChatEmpty();

    private void CheckConversationType()
    {
        IsContinuousConversation = ConversationType == ConversationType.Continuous;
        IsSingleConversation = ConversationType == ConversationType.Single;
        IsContextConversation = ConversationType == ConversationType.Context;
    }

    partial void OnConversationTypeChanged(ConversationType value)
    {
        CheckConversationType();
        if (IsContextConversation)
        {
            UseNewMetadata(default);
        }
        else
        {
            _settingsToolkit.WriteLocalSetting(SettingNames.LastConversationType, value);
        }
    }
}
