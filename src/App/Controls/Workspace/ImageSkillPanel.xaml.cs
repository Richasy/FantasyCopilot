// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App.Workspace;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls.Workspace;

/// <summary>
/// Image skill panel.
/// </summary>
public sealed partial class ImageSkillPanel : ImageSkillPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageSkillPanel"/> class.
    /// </summary>
    public ImageSkillPanel()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<IImageSkillsModuleViewModel>();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(default);

    private void OnAddButtonClickAsync(object sender, RoutedEventArgs e)
        => ViewModel.EditConfigCommand.Execute(default);

    private async void OnDeleteButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement)?.DataContext as ImageSkillConfig;
        var dialog = new DeleteDialog(StringNames.DeleteImageSkill, StringNames.DeleteImageSkillTip)
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
        var data = (sender as FrameworkElement)?.DataContext as ImageSkillConfig;
        ViewModel.EditConfigCommand.Execute(data);
    }
}

/// <summary>
/// Base for <see cref="ImageSkillPanel"/>.
/// </summary>
public class ImageSkillPanelBase : ReactiveUserControl<IImageSkillsModuleViewModel>
{
}
