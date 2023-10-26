// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Input;
using RichasyAssistant.DI.Container;
using RichasyAssistant.ViewModels.Interfaces;
using Windows.UI.Core;

namespace RichasyAssistant.App.Controls.Sessions;

/// <summary>
/// Session panel.
/// </summary>
public sealed partial class SessionPanel : SessionPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SessionPanel"/> class.
    /// </summary>
    public SessionPanel()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <summary>
    /// Option button is clicked.
    /// </summary>
    public event EventHandler OptionButtonClick;

    /// <summary>
    /// New session button is clicked.
    /// </summary>
    public event EventHandler NewSessionButtonClick;

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is ISessionViewModel oldVM)
        {
            oldVM.RequestScrollToBottom -= OnRequestScrollToBottomAsync;
        }

        if (e.NewValue == null)
        {
            return;
        }

        var vm = e.NewValue as ISessionViewModel;
        vm.RequestScrollToBottom += OnRequestScrollToBottomAsync;
        if (IsLoaded)
        {
            ResetElementState();
        }
    }

    private async void OnRequestScrollToBottomAsync(object sender, EventArgs e)
    {
        await Task.Delay(200);
        MessageScrollView.ChangeView(0, MessageScrollView.ScrollableHeight + MessageScrollView.ActualHeight, 1);
    }

    private void OnChatTypeSegmentedSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ViewModel == null)
        {
            return;
        }

        var selectedIndex = ChatTypeSegmented.SelectedIndex;
        ViewModel.ConversationType = (ConversationType)selectedIndex;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ResetElementState();

    private async void OnDeleteSessionButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var dialog = new DeleteDialog(StringNames.DeleteSession, StringNames.DeleteSessionTip)
        {
            XamlRoot = XamlRoot,
        };
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            ViewModel.DeleteCommand.Execute(default);
            Locator.Current.GetService<IChatSessionPageViewModel>().AbandonSessionCommand.Execute(ViewModel.Id);
        }
    }

    private void OnInputBoxKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
            var isShiftDown = shiftState == CoreVirtualKeyStates.Down
                || shiftState == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked);
            if (!isShiftDown)
            {
                e.Handled = true;
                ViewModel.SendMessageCommand.Execute(default);
            }
        }
    }

    private void ResetElementState()
    {
        InputBox.Focus(FocusState.Programmatic);
        ChatTypeSegmented.SelectedIndex = ViewModel == null ? -1 : (int)ViewModel.ConversationType;
    }

    private void OnOptionButtonClick(object sender, RoutedEventArgs e)
        => OptionButtonClick?.Invoke(this, EventArgs.Empty);

    private async void OnSaveButtonClickAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsLocalSession)
        {
            ViewModel.SaveCommand.Execute(default);
        }
        else
        {
            var dialog = new SessionSaveDialog(ViewModel)
            {
                XamlRoot = XamlRoot,
            };
            await dialog.ShowAsync();
        }
    }

    private void OnNewSessionButtonClick(object sender, RoutedEventArgs e)
        => NewSessionButtonClick?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// Base of <see cref="SessionPanel"/>.
/// </summary>
public class SessionPanelBase : ReactiveUserControl<ISessionViewModel>
{
}
