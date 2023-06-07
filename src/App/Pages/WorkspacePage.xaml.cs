// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Pages;

/// <summary>
/// Workspace page.
/// </summary>
public sealed partial class WorkspacePage : WorkspacePageBase
{
    private readonly ISemanticSkillsModuleViewModel _semanticSkillsVM;
    private readonly IImageSkillsModuleViewModel _imageSkillsVM;
    private readonly IWorkflowsModuleViewModel _workflowsVM;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkspacePage"/> class.
    /// </summary>
    public WorkspacePage()
    {
        InitializeComponent();
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        _semanticSkillsVM = Locator.Current.GetService<ISemanticSkillsModuleViewModel>();
        _imageSkillsVM = Locator.Current.GetService<IImageSkillsModuleViewModel>();
        _workflowsVM = Locator.Current.GetService<IWorkflowsModuleViewModel>();
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        ModulePicker.SelectedIndex = (int)ViewModel.SelectedType;
        CoreViewModel.BackRequest += OnCoreViewModelBackRequest;
        CoreViewModel.IsBackButtonShown =
            _semanticSkillsVM.IsEditing
            || _imageSkillsVM.IsEditing
            || _workflowsVM.IsEditing
            || _workflowsVM.IsRunning;
    }

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
        => CoreViewModel.BackRequest -= OnCoreViewModelBackRequest;

    private void OnModulePickerSelectionChanged(object sender, SelectionChangedEventArgs e)
        => ViewModel.SelectedType = (WorkspaceDataType)ModulePicker.SelectedIndex;

    private void OnCoreViewModelBackRequest(object sender, EventArgs e)
    {
        if (ViewModel.SelectedType == WorkspaceDataType.SemanticSkills)
        {
            _semanticSkillsVM.IsEditing = false;
        }
        else if (ViewModel.SelectedType == WorkspaceDataType.ImageSkills)
        {
            _imageSkillsVM.IsEditing = false;
        }
        else if (ViewModel.SelectedType == WorkspaceDataType.Workflows)
        {
            _workflowsVM.IsEditing = false;
            _workflowsVM.IsRunning = false;
        }
    }
}

/// <summary>
/// Base for <see cref="WorkspacePage"/>.
/// </summary>
public class WorkspacePageBase : PageBase<IWorkspacePageViewModel>
{
}
