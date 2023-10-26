// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichasyAssistant.Models.App;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// View model base.
/// </summary>
public class ViewModelBase : ObservableObject
{
    /// <summary>
    /// Add callback for <see cref="AsyncRelayCommand.IsRunning"/> property of async commands.
    /// </summary>
    /// <param name="handler">Callback.</param>
    /// <param name="commands">Commands.</param>
    protected static void AttachIsRunningToAsyncCommand(Action<bool> handler, params IAsyncRelayCommand[] commands)
    {
        foreach (var command in commands)
        {
            command.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AsyncRelayCommand.IsRunning))
                {
                    handler(command.IsRunning);
                }
            };
        }
    }

    /// <summary>
    /// Try clear collection.
    /// </summary>
    /// <typeparam name="T">Item type.</typeparam>
    /// <param name="collection">Collection.</param>
    protected static void TryClear<T>(ObservableCollection<T> collection)
    {
        try
        {
            collection.Clear();
        }
        catch (Exception)
        {
            // Do nothing.
        }
    }

    /// <summary>
    /// Try clear collection.
    /// </summary>
    /// <typeparam name="T">Item type.</typeparam>
    /// <param name="collection">Collection.</param>
    protected static void TryClear<T>(SynchronizedObservableCollection<T> collection)
    {
        if (collection.Count > 0)
        {
            collection.Clear();
        }
    }
}
