// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls.Knowledge;

/// <summary>
/// Knowledge context item.
/// </summary>
public sealed partial class KnowledgeContextItem : KnowledgeContextItemBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgeContextItem"/> class.
    /// </summary>
    public KnowledgeContextItem()
        => InitializeComponent();

    private void OnFileNameTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        => ViewModel.IsSelected = !ViewModel.IsSelected;
}

/// <summary>
/// Base for <see cref="KnowledgeContextItem"/>.
/// </summary>
public class KnowledgeContextItemBase : ReactiveUserControl<IKnowledgeContextViewModel>
{
}
