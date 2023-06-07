// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Settings;

/// <summary>
/// Setting section base.
/// </summary>
public class SettingSectionBase : ReactiveUserControl<ISettingsPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingSectionBase"/> class.
    /// </summary>
    public SettingSectionBase()
        => ViewModel = Locator.Current.GetService<ISettingsPageViewModel>();
}
