// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Connector;

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
        Unloaded += OnUnloaded;
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is IConnectorConfigViewModel oldVM)
        {
            oldVM.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (e.NewValue is IConnectorConfigViewModel newVM)
        {
            newVM.PropertyChanged += OnViewModelPropertyChanged;
        }
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
        if(e.PropertyName == nameof(IConnectorConfigViewModel.))
    }
}

/// <summary>
/// Base class for <see cref="ConnectorItem"/>.
/// </summary>
public class ConnectorItemBase : ReactiveUserControl<IConnectorConfigViewModel>
{
}
