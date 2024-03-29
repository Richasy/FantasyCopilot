﻿// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls.Connector;

/// <summary>
/// Connector item.
/// </summary>
public sealed partial class ConnectorItem : ConnectorItemBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectorItem"/> class.
    /// </summary>
    public ConnectorItem()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is IConnectorConfigViewModel newVM)
        {
            CheckState();
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        LogTextBlock.Text = ViewModel.LogContent?.Trim() ?? string.Empty;
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        CheckState();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IConnectorConfigViewModel.State))
        {
            CheckState();
        }
        else if (e.PropertyName == nameof(IConnectorConfigViewModel.LogContent))
        {
            LogTextBlock.Text = ViewModel.LogContent.Trim();
        }
    }

    private void CheckState()
    {
        switch (ViewModel.State)
        {
            case ConnectorState.NotStarted:
                VisualStateManager.GoToState(this, nameof(DisconnectedState), false);
                break;
            case ConnectorState.Launching:
                VisualStateManager.GoToState(this, nameof(ConnectingState), false);
                break;
            case ConnectorState.Connected:
                VisualStateManager.GoToState(this, nameof(ConnectedState), false);
                break;
            default:
                break;
        }
    }
}

/// <summary>
/// Base class for <see cref="ConnectorItem"/>.
/// </summary>
public class ConnectorItemBase : ReactiveUserControl<IConnectorConfigViewModel>
{
}
