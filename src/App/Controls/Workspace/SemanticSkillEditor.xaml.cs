// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.DI.Container;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls.Workspace;

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
