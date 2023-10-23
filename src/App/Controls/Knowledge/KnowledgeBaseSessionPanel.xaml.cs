// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Input;
using RichasyAssistant.DI.Container;
using RichasyAssistant.ViewModels.Interfaces;
using Windows.UI.Core;

namespace RichasyAssistant.App.Controls.Knowledge;

/// <summary>
/// Knowledge base session panel.
/// </summary>
public sealed partial class KnowledgeBaseSessionPanel : KnowledgeBaseSessionPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgeBaseSessionPanel"/> class.
    /// </summary>
    public KnowledgeBaseSessionPanel()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<IKnowledgeBaseSessionViewModel>();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
        => ViewModel.RequestScrollToBottom -= OnRequestScrollToBottomAsync;

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        KnowledgeSearchTypeSegmented.SelectedIndex = (int)ViewModel.SearchType;
        CheckItemFocus();
        ViewModel.RequestScrollToBottom += OnRequestScrollToBottomAsync;
    }

    private async void OnRequestScrollToBottomAsync(object sender, EventArgs e)
    {
        await Task.Delay(200);
        if (ViewModel.IsQuickQA)
        {
            MessageScrollViewer.ChangeView(0, MessageScrollViewer.ScrollableHeight + MessageScrollViewer.ActualHeight, 1);
        }
        else
        {
            ContextScrollViewer.ChangeView(0, ContextScrollViewer.ScrollableHeight + ContextScrollViewer.ActualHeight, 1);
        }
    }

    private void OnKnowledgeSearchTypeSegmentedSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded || KnowledgeSearchTypeSegmented.SelectedIndex == -1)
        {
            return;
        }

        var type = (KnowledgeSearchType)KnowledgeSearchTypeSegmented.SelectedIndex;
        ViewModel.SearchType = type;
        CheckItemFocus();
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
                ViewModel.SendQueryCommand.Execute(default);
            }
        }
    }

    private void CheckItemFocus()
    {
        if (ViewModel.IsQuickQA)
        {
            QuickInputBox.Focus(FocusState.Programmatic);
        }
        else
        {
            AdvancedInputBox.Focus(FocusState.Programmatic);
        }
    }
}

/// <summary>
/// Base for <see cref="KnowledgeBaseSessionPanel"/>.
/// </summary>
public class KnowledgeBaseSessionPanelBase : ReactiveUserControl<IKnowledgeBaseSessionViewModel>
{
}
