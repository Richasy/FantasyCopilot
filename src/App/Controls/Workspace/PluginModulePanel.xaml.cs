// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Workspace;

/// <summary>
/// Plugin module panel.
/// </summary>
public sealed partial class PluginModulePanel : PluginModulePanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginModulePanel"/> class.
    /// </summary>
    public PluginModulePanel()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<IPluginsModuleViewModel>();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// Base for <see cref="PluginModulePanel"/>.
/// </summary>
public class PluginModulePanelBase : ReactiveUserControl<IPluginsModuleViewModel>
{
}
