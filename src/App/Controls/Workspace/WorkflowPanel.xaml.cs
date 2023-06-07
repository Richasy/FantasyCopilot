// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Workspace;

/// <summary>
/// Workflow panel.
/// </summary>
public sealed partial class WorkflowPanel : WorkflowPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkflowPanel"/> class.
    /// </summary>
    public WorkflowPanel()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<IWorkflowsModuleViewModel>();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(default);

    private void OnAddButtonClickAsync(object sender, RoutedEventArgs e)
        => ViewModel.EditConfigCommand.Execute(default);

    private async void OnDeleteButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement)?.DataContext as WorkflowMetadata;
        var dialog = new DeleteDialog(StringNames.DeleteWorkflow, StringNames.DeleteWorkflowTip)
        {
            XamlRoot = XamlRoot,
        };
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            ViewModel.DeleteConfigCommand.Execute(data.Id);
        }
    }

    private void OnItemClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement)?.DataContext as WorkflowMetadata;
        ViewModel.RunConfigCommand.Execute(data);
    }

    private void OnModifyButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement)?.DataContext as WorkflowMetadata;
        ViewModel.EditConfigCommand.Execute(data);
    }
}

/// <summary>
/// Base for <see cref="WorkflowPanel"/>.
/// </summary>
public class WorkflowPanelBase : ReactiveUserControl<IWorkflowsModuleViewModel>
{
}
