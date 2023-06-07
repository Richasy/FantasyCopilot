// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.UI.Input;
using Windows.UI.Core;

namespace FantasyCopilot.App.Pages;

/// <summary>
/// Translate page.
/// </summary>
public sealed partial class TranslatePage : TranslatePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatePage"/> class.
    /// </summary>
    public TranslatePage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.IsBackButtonShown = false;
        if (CoreViewModel.IsTranslateAvailable)
        {
            ViewModel.InitializeCommand.Execute(default);
        }

        SourceBox?.Focus(FocusState.Programmatic);
    }

    private void OnSourceBoxKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
            var isShiftDown = shiftState == CoreVirtualKeyStates.Down
                || shiftState == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked);
            if (!isShiftDown)
            {
                e.Handled = true;
                OutputBox.Focus(FocusState.Programmatic);
                ViewModel.TranslateCommand.Execute(default);
            }
        }
    }
}

/// <summary>
/// Base for <see cref="TranslatePage"/>.
/// </summary>
public class TranslatePageBase : PageBase<ITranslatePageViewModel>
{
}
