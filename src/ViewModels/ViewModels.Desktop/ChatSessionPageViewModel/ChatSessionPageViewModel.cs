// Copyright (c) Richasy Assistant. All rights reserved.

using CommunityToolkit.Mvvm.Input;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Services.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

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

        CheckChatConnectorAvailable();
        CheckConnectorStreamOption();
    }

    private static void CheckChatConnectorAvailable()
    {
        var appVM = Locator.Current.GetService<IAppViewModel>();
        if (appVM.ConnectorGroup.ContainsKey(ConnectorType.Chat))
        {
            var chatConnector = appVM.ConnectorGroup[ConnectorType.Chat];
            if (!chatConnector.IsLaunched)
            {
                chatConnector.LaunchCommand.Execute(default);
            }
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
        if (appVM.ConnectorGroup.Count > 0)
        {
            if (appVM.ConnectorGroup.ContainsKey(ConnectorType.Chat))
            {
                var connector = appVM.ConnectorGroup[ConnectorType.Chat];
                CurrentSession.Options.IsStreamOutputEnabled = connector.SupportChatStream;
            }
            else
            {
                CurrentSession.Options.IsStreamOutputEnabled = false;
            }
        }
        else
        {
            CurrentSession.Options.IsStreamOutputEnabled = true;
        }

        var sessionService = Locator.Current.GetService<ISessionService>();
        sessionService.UpdateSessionOptions(CurrentSession?.Options.GetOptions());
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
