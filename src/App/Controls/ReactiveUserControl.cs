// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.App.Controls;

/// <summary>
/// Reactive user control base.
/// </summary>
/// <typeparam name="TViewModel">View model.</typeparam>
public class ReactiveUserControl<TViewModel> : UserControl
    where TViewModel : class
{
    /// <summary>
    /// <see cref="ViewModel"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty
            .Register(nameof(ViewModel), typeof(TViewModel), typeof(ReactiveUserControl<TViewModel>), new PropertyMetadata(default, new PropertyChangedCallback((dp, args) =>
            {
                var instance = dp as ReactiveUserControl<TViewModel>;
                instance.OnViewModelChanged(args);
            })));

    /// <summary>
    /// View model.
    /// </summary>
    public TViewModel ViewModel
    {
        get => (TViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// 当 <see cref="ViewModel"/> 改变时调用该方法，可由派生类重写.
    /// </summary>
    /// <param name="e">依赖属性更改事件参数.</param>
    internal virtual void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        // Do nothing.
    }
}
