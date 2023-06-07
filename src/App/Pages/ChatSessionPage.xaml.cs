// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.UI.Xaml.Navigation;

namespace FantasyCopilot.App.Pages;

/// <summary>
/// New session page.
/// </summary>
public sealed partial class ChatSessionPage : ChatSessionPageBase
{
    private ISessionViewModel _tempVM;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionPage"/> class.
    /// </summary>
    public ChatSessionPage()
        => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is ISessionViewModel vm)
        {
            _tempVM = vm;
        }
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        if (CoreViewModel.IsChatAvailable)
        {
            if (_tempVM != null)
            {
                ViewModel.Initialize(_tempVM);
                _tempVM = null;
            }
            else
            {
                ViewModel.Initialize();
            }
        }

        CoreViewModel.BackRequest += OnBackRequest;
        CoreViewModel.IsBackButtonShown = ViewModel.IsInSettings;
        ViewModel.CurrentSession?.ConnectToSessionServiceCommand.Execute(default);
    }

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
    {
        CoreViewModel.BackRequest -= OnBackRequest;
        ViewModel.CurrentSession?.DisconnectFromSessionServiceCommand.Execute(default);
    }

    private void OnBackRequest(object sender, EventArgs e)
        => ViewModel.IsInSettings = false;

    private void OnNewSessionButtonClick(object sender, EventArgs e)
    {
        var newSession = Locator.Current.GetService<ISessionViewModel>();
        newSession.Initialize();
        ViewModel.Initialize(newSession);
    }

    private void OnOptionButtonClick(object sender, EventArgs e)
        => ViewModel.IsInSettings = true;
}

/// <summary>
/// Base of <see cref="ChatSessionPage"/>.
/// </summary>
public class ChatSessionPageBase : PageBase<IChatSessionPageViewModel>
{
}
