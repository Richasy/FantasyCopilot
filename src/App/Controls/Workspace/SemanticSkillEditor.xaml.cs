// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Workspace;

/// <summary>
/// Semantic skill editor.
/// </summary>
public sealed partial class SemanticSkillEditor : SemanticSkillEditorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticSkillEditor"/> class.
    /// </summary>
    public SemanticSkillEditor()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<ISemanticSkillEditModuleViewModel>();
    }
}

/// <summary>
/// Base for <see cref="SemanticSkillEditor"/>.
/// </summary>
public class SemanticSkillEditorBase : ReactiveUserControl<ISemanticSkillEditModuleViewModel>
{
}
