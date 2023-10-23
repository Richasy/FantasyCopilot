// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.DI.Container;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls.Settings;

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
