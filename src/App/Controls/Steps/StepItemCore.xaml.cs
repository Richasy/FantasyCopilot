// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using FantasyCopilot.DI.Container;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Steps;

/// <summary>
/// Step item base.
/// </summary>
public sealed partial class StepItemCore : WorkflowStepControlBase
{
    /// <summary>
    /// Dependency property of <see cref="ConfigurationElement"/>.
    /// </summary>
    public static readonly DependencyProperty ConfigurationElementProperty =
        DependencyProperty.Register(nameof(ConfigurationElement), typeof(object), typeof(StepItemCore), new PropertyMetadata(default));

    /// <summary>
    /// Dependency property of <see cref="FinishElement"/>.
    /// </summary>
    public static readonly DependencyProperty FinishElementProperty =
        DependencyProperty.Register(nameof(FinishElement), typeof(object), typeof(StepItemCore), new PropertyMetadata(default));

    /// <summary>
    /// Dependency property of <see cref="InputElement"/>.
    /// </summary>
    public static readonly DependencyProperty InputElementProperty =
        DependencyProperty.Register(nameof(InputElement), typeof(object), typeof(StepItemCore), new PropertyMetadata(default));

    /// <summary>
    /// Dependency property of <see cref="OutputElement"/>.
    /// </summary>
    public static readonly DependencyProperty OutputElementProperty =
        DependencyProperty.Register(nameof(OutputElement), typeof(object), typeof(StepItemCore), new PropertyMetadata(default));

    /// <summary>
    /// Dependency property of <see cref="StepName"/>.
    /// </summary>
    public static readonly DependencyProperty StepNameProperty =
        DependencyProperty.Register(nameof(StepName), typeof(string), typeof(StepItemCore), new PropertyMetadata(default));

    /// <summary>
    /// Dependency property of <see cref="StepDescription"/>.
    /// </summary>
    public static readonly DependencyProperty StepDescriptionProperty =
        DependencyProperty.Register(nameof(StepDescription), typeof(string), typeof(StepItemCore), new PropertyMetadata(default));

    /// <summary>
    /// Dependency property of <see cref="IsConfigMoreOptionsButtonShown"/>.
    /// </summary>
    public static readonly DependencyProperty IsConfigMoreOptionsButtonShownProperty =
        DependencyProperty.Register(nameof(IsConfigMoreOptionsButtonShown), typeof(bool), typeof(StepItemCore), new PropertyMetadata(true));

    /// <summary>
    /// Dependency property of <see cref="IsMoveDownwardEnabled"/>.
    /// </summary>
    public static readonly DependencyProperty IsMoveDownwardEnabledProperty =
        DependencyProperty.Register(nameof(IsMoveDownwardEnabled), typeof(bool), typeof(StepItemCore), new PropertyMetadata(true));

    /// <summary>
    /// Dependency property of <see cref="IsMoveUpwardEnabled"/>.
    /// </summary>
    public static readonly DependencyProperty IsMoveUpwardEnabledProperty =
        DependencyProperty.Register(nameof(IsMoveUpwardEnabled), typeof(bool), typeof(StepItemCore), new PropertyMetadata(true));

    /// <summary>
    /// Initializes a new instance of the <see cref="StepItemCore"/> class.
    /// </summary>
    public StepItemCore()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <summary>
    /// The control displayed in the configuration state.
    /// </summary>
    public object ConfigurationElement
    {
        get => GetValue(ConfigurationElementProperty);
        set => SetValue(ConfigurationElementProperty, value);
    }

    /// <summary>
    /// The control displayed in the completed state.
    /// </summary>
    public object FinishElement
    {
        get => GetValue(FinishElementProperty);
        set => SetValue(FinishElementProperty, value);
    }

    /// <summary>
    /// The control displayed in the input state.
    /// </summary>
    public object InputElement
    {
        get => GetValue(InputElementProperty);
        set => SetValue(InputElementProperty, value);
    }

    /// <summary>
    /// The control displayed in the output state.
    /// </summary>
    public object OutputElement
    {
        get => GetValue(OutputElementProperty);
        set => SetValue(OutputElementProperty, value);
    }

    /// <summary>
    /// Step name.
    /// </summary>
    public string StepName
    {
        get => (string)GetValue(StepNameProperty);
        set => SetValue(StepNameProperty, value);
    }

    /// <summary>
    /// Step description.
    /// </summary>
    public string StepDescription
    {
        get => (string)GetValue(StepDescriptionProperty);
        set => SetValue(StepDescriptionProperty, value);
    }

    /// <summary>
    /// Whether to display the option button in the configuration state.
    /// </summary>
    public bool IsConfigMoreOptionsButtonShown
    {
        get => (bool)GetValue(IsConfigMoreOptionsButtonShownProperty);
        set => SetValue(IsConfigMoreOptionsButtonShownProperty, value);
    }

    /// <summary>
    /// Whether to allow downward movement.
    /// </summary>
    public bool IsMoveDownwardEnabled
    {
        get => (bool)GetValue(IsMoveDownwardEnabledProperty);
        set => SetValue(IsMoveDownwardEnabledProperty, value);
    }

    /// <summary>
    /// Whether to allow upward movement.
    /// </summary>
    public bool IsMoveUpwardEnabled
    {
        get => (bool)GetValue(IsMoveUpwardEnabledProperty);
        set => SetValue(IsMoveUpwardEnabledProperty, value);
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is IWorkflowStepViewModel oldVM)
        {
            oldVM.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (e.NewValue is IWorkflowStepViewModel newVM)
        {
            newVM.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.Index))
        {
            CheckMenu();
        }
        else if (e.PropertyName == nameof(ViewModel.State))
        {
            CheckState();
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        CheckState();
        CheckMenu();
    }

    private void CheckState()
    {
        if (ViewModel == null)
        {
            return;
        }

        var stateName = ViewModel.State.ToString() + "State";
        VisualStateManager.GoToState(this, stateName, false);
    }

    private void CheckMenu()
    {
        if (ViewModel == null)
        {
            return;
        }

        if (ViewModel.State == WorkflowStepState.Configuring && IsConfigMoreOptionsButtonShown)
        {
            var editorVM = Locator.Current.GetService<IWorkflowEditorViewModel>();
            IsMoveUpwardEnabled = ViewModel.Index > 0;
            IsMoveDownwardEnabled = ViewModel.Index < editorVM.Steps.Count - 1;
        }
    }

    private void OnMoveUpwardItemClick(object sender, RoutedEventArgs e)
        => Locator.Current.GetService<IWorkflowEditorViewModel>()
            .MoveUpwardCommand.Execute(ViewModel);

    private void OnMoveDownwardItemClick(object sender, RoutedEventArgs e)
        => Locator.Current.GetService<IWorkflowEditorViewModel>()
            .MoveDownwardCommand.Execute(ViewModel);

    private void OnDeleteItemClick(object sender, RoutedEventArgs e)
    {
        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        Locator.Current.GetService<IWorkflowEditorViewModel>()
            .RemoveStepCommand.Execute(ViewModel.Index);
    }
}
