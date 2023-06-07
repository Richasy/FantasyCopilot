// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Workspace;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Workspace;

/// <summary>
/// Workflow editor.
/// </summary>
public sealed partial class WorkflowEditor : WorkflowEditorBase
{
    private IResourceToolkit _resourceToolkit;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkflowEditor"/> class.
    /// </summary>
    public WorkflowEditor()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<IWorkflowEditorViewModel>();
        InitializeInputItems();
        InitializeOutputItems();
        InspireButton.Translation += new System.Numerics.Vector3(2, 2, 20);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
        => ViewModel.StepCommandsLoaded -= OnStepCommandsLoaded;

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _resourceToolkit ??= Locator.Current.GetService<IResourceToolkit>();
        ViewModel.StepCommandsLoaded -= OnStepCommandsLoaded;
        ViewModel.StepCommandsLoaded += OnStepCommandsLoaded;
        if (ViewModel.StepCommands != null && ViewModel.StepCommands.Count > 0)
        {
            InitializeStepItems();
        }
    }

    private void OnStepCommandsLoaded(object sender, EventArgs e)
        => InitializeStepItems();

    private void InitializeStepItems()
    {
        StepFlyout.Items.Clear();
        foreach (var item in ViewModel.StepCommands)
        {
            AddStepMenuItem(item);
        }
    }

    private void InitializeInputItems()
    {
        foreach (var item in ViewModel.InputCommands)
        {
            var menuItem = new MenuFlyoutItem
            {
                Text = item.Name,
                DataContext = item,
                MinWidth = 160,
            };
            menuItem.Click += OnInputMenuItemClick;
            InputFlyout.Items.Add(menuItem);
        }
    }

    private void InitializeOutputItems()
    {
        foreach (var item in ViewModel.OutputCommands)
        {
            var menuItem = new MenuFlyoutItem
            {
                Text = item.Name,
                DataContext = item,
                MinWidth = 160,
            };
            menuItem.Click += OnOutputMenuItemClick;
            OutputFlyout.Items.Add(menuItem);
        }
    }

    private void AddStepMenuItem(WorkCommandBase command, MenuFlyoutSubItem targetObject = null)
    {
        if (command is WorkCommandItem item)
        {
            var menuItem = new MenuFlyoutItem
            {
                Text = item.Name,
                DataContext = item,
                MinWidth = 160,
            };

            if (item.Skill == SkillType.PluginCommand)
            {
                menuItem.KeyboardAcceleratorTextOverride = _resourceToolkit.GetLocalizedString(StringNames.Plugins);
            }

            menuItem.Click += OnStepMenuItemClick;
            if (targetObject == null)
            {
                StepFlyout.Items.Add(menuItem);
            }
            else
            {
                targetObject.Items.Add(menuItem);
            }
        }
        else if (command is WorkCommandGroup group)
        {
            var subItem = new MenuFlyoutSubItem
            {
                Text = group.Name,
            };
            foreach (var subCommand in group.Commands)
            {
                AddStepMenuItem(subCommand, subItem);
            }

            if (targetObject == null)
            {
                StepFlyout.Items.Add(subItem);
            }
            else
            {
                targetObject.Items.Add(subItem);
            }
        }
    }

    private void OnStepMenuItemClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement)?.DataContext as WorkCommandItem;
        ViewModel.CreateStepCommand.Execute(data);
    }

    private void OnInputMenuItemClick(object sender, RoutedEventArgs e)
    {
        var data = (WorkCommandItem)((MenuFlyoutItem)sender).DataContext;
        ViewModel.CreateInputCommand.Execute(data.Skill);
    }

    private void OnOutputMenuItemClick(object sender, RoutedEventArgs e)
    {
        var data = (WorkCommandItem)((MenuFlyoutItem)sender).DataContext;
        ViewModel.CreateOutputCommand.Execute(data.Skill);
    }

    private void OnResetInputButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.Input = default;

    private void OnResetOutputButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.Output = default;
}

/// <summary>
/// Workflow editor component.
/// </summary>
public class WorkflowEditorBase : ReactiveUserControl<IWorkflowEditorViewModel>
{
}
