// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Workspace;

/// <summary>
/// Plugin command item.
/// </summary>
public sealed partial class PluginCommandItem : PluginCommandItemBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginCommandItem"/> class.
    /// </summary>
    public PluginCommandItem() => InitializeComponent();
}

/// <summary>
/// Base for <see cref="PluginCommandItem"/>.
/// </summary>
public class PluginCommandItemBase : ReactiveUserControl<IPluginCommandItemViewModel>
{
}
