// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PromptsAndSessionsPage : PromptsAndSessionsPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptsAndSessionsPage"/> class.
    /// </summary>
    public PromptsAndSessionsPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.IsBackButtonShown = false;
        ModulePicker.SelectedIndex = (int)ViewModel.SelectedType;
    }

    private void OnModulePickerSelectionChanged(object sender, SelectionChangedEventArgs e)
        => ViewModel.SelectedType = (ChatDataType)ModulePicker.SelectedIndex;
}

/// <summary>
/// Base of <see cref="PromptsAndSessionsPage"/>.
/// </summary>
public class PromptsAndSessionsPageBase : PageBase<IPromptsAndSessionsPageViewModel>
{
}
