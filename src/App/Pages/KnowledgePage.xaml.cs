// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Pages;

/// <summary>
/// Knowledge base page.
/// </summary>
public sealed partial class KnowledgePage : KnowledgePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgePage"/> class.
    /// </summary>
    public KnowledgePage()
        => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.BackRequest += OnBackRequest;
        CoreViewModel.IsBackButtonShown = ViewModel.CurrentBase != null;
        if (CoreViewModel.IsKnowledgeAvailable)
        {
            ViewModel.InitializeCommand.Execute(default);
        }
    }

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
        => CoreViewModel.BackRequest -= OnBackRequest;

    private void OnBackRequest(object sender, EventArgs e)
        => ViewModel.ExitBaseCommand.Execute(default);
}

/// <summary>
/// Base for <see cref="KnowledgePage"/>.
/// </summary>
public class KnowledgePageBase : PageBase<IKnowledgePageViewModel>
{
}
