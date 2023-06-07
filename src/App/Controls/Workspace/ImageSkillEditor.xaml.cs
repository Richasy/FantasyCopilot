// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Workspace;

/// <summary>
/// Image skill editor.
/// </summary>
public sealed partial class ImageSkillEditor : ImageSkillEditorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageSkillEditor"/> class.
    /// </summary>
    public ImageSkillEditor()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<IImageSkillEditModuleViewModel>();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// Base for <see cref="ImageSkillEditor"/>.
/// </summary>
public class ImageSkillEditorBase : ReactiveUserControl<IImageSkillEditModuleViewModel>
{
}
