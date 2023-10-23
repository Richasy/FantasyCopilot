// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App.Workspace;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls.Workspace;

/// <summary>
/// Workflow panel.
/// </summary>
public sealed partial class WorkflowPanel : WorkflowPanelBase
{
    private bool _isInitialized = false;

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
    {
        if (_isInitialized)
        {
            return;
        }

        ViewModel.InitializeCommand.Execute(default);
        _isInitialized = true;
    }

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

    private async void OnExportButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var settingsToolkit = Locator.Current.GetService<ISettingsToolkit>();
        var shouldSkipTip = settingsToolkit.ReadLocalSetting(SettingNames.ShouldSkipWorkflowExportTip, false);
        if (!shouldSkipTip)
        {
            var resourceToolkit = Locator.Current.GetService<IResourceToolkit>();
            var dialog = new TipDialog(resourceToolkit.GetLocalizedString(StringNames.WorkflowExportTip))
            {
                XamlRoot = XamlRoot,
            };

            await dialog.ShowAsync();
            settingsToolkit.WriteLocalSetting(SettingNames.ShouldSkipWorkflowExportTip, true);
        }
    }
}

/// <summary>
/// Base for <see cref="WorkflowPanel"/>.
/// </summary>
public class WorkflowPanelBase : ReactiveUserControl<IWorkflowsModuleViewModel>
{
}
