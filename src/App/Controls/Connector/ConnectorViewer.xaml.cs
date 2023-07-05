﻿// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Linq;
using FantasyCopilot.DI.Container;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Connector;

/// <summary>
/// Connector viewer.
/// </summary>
public sealed partial class ConnectorViewer : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectorViewer"/> class.
    /// </summary>
    public ConnectorViewer()
        => InitializeComponent();

    /// <summary>
    /// Initialize connectors.
    /// </summary>
    public void Initialize()
    {
        var appVM = Locator.Current.GetService<IAppViewModel>();
        var connectors = appVM.ConnectorGroup.Select(p => p.Value).Distinct();
        ConnectorRepeater.ItemsSource = connectors;
    }
}
