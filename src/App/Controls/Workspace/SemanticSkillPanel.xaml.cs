// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Workspace;

/// <summary>
/// Semantic skill panel.
/// </summary>
public sealed partial class SemanticSkillPanel : SemanticSkillPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticSkillPanel"/> class.
    /// </summary>
    public SemanticSkillPanel()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<ISemanticSkillsModuleViewModel>();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(default);

    private void OnAddButtonClickAsync(object sender, RoutedEventArgs e)
        => ViewModel.EditConfigCommand.Execute(default);

    private async void OnDeleteButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement)?.DataContext as SemanticSkillConfig;
        var dialog = new DeleteDialog(StringNames.DeleteSemanticSkill, StringNames.DeleteSemanticSkillTip)
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
        var data = (sender as FrameworkElement)?.DataContext as SemanticSkillConfig;
        ViewModel.EditConfigCommand.Execute(data);
    }
}

/// <summary>
/// Base for <see cref="SemanticSkillPanel"/>.
/// </summary>
public class SemanticSkillPanelBase : ReactiveUserControl<ISemanticSkillsModuleViewModel>
{
}
