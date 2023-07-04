﻿// Copyright (c) Fantasy Copilot. All rights reserved.

using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Chat session page view model.
/// </summary>
public sealed partial class ChatSessionPageViewModel : ViewModelBase, IChatSessionPageViewModel
{
    /// <inheritdoc/>
    public void Initialize(ISessionViewModel session = default)
    {
        CurrentSession?.DisconnectFromSessionServiceCommand.Execute(default);

        if (session == null && CurrentSession == null)
        {
            var newVM = Locator.Current.GetService<ISessionViewModel>();
            newVM.Initialize();
            CurrentSession = newVM;
        }
        else if (session != null)
        {
            CurrentSession = session;
        }
    }

    [RelayCommand]
    private void AbandonSession(string id)
    {
        CurrentSession?.DisconnectFromSessionServiceCommand.Execute(default);

        if (id == AppConstants.ClearSessionId)
        {
            CurrentSession = default;
        }
        else if (CurrentSession?.Id == id)
        {
            var newVM = Locator.Current.GetService<ISessionViewModel>();
            newVM.Initialize();
            CurrentSession = newVM;
        }

        CheckConnectorStreamOption();
    }

    private void CheckConnectorStreamOption()
    {
        var appVM = Locator.Current.GetService<IAppViewModel>();
        if (appVM.ConnectorGroup.ContainsKey(ConnectorType.Chat))
        {
            var connector = appVM.ConnectorGroup[ConnectorType.Chat];
            CurrentSession.Options.IsStreamOutputEnabled = connector.SupportChatStream;
        }
    }

    partial void OnIsInSettingsChanged(bool value)
    {
        var appVM = Locator.Current.GetService<IAppViewModel>();
        appVM.IsBackButtonShown = value;

        if (!value)
        {
            // Update cache when hide settings.
            var sessionService = Locator.Current.GetService<ISessionService>();
            sessionService.UpdateSessionOptions(CurrentSession?.Options.GetOptions());
        }
    }
}
