// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.TemplateEngine.Prompt;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Workspace.Steps;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.NativeSkills;

/// <summary>
/// Some functions of the app.
/// </summary>
public sealed class NativeSkill
{
    private readonly WorkflowContext _workflowContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="NativeSkill"/> class.
    /// </summary>
    public NativeSkill()
        => _workflowContext = Locator.Current.GetVariable<WorkflowContext>();

    /// <summary>
    /// Send notifications to the user.
    /// </summary>
    /// <param name="context">Current context.</param>
    /// <returns><see cref="Task"/>.</returns>
    [SKName(WorkflowConstants.Native.TextNotificationName)]
    [Description(WorkflowConstants.Native.TextNotificationDescription)]
    [SKFunction]
    public async Task<SKContext> SendNotificationAsync(SKContext context)
    {
        var parameters = _workflowContext.GetStepParameters<TextNotificationStep>();
        if (parameters == null || string.IsNullOrEmpty(parameters.Text))
        {
            throw new SKException("No notification parameters found.");
        }

        var engine = new PromptTemplateEngine();
        var text = await engine.RenderAsync(parameters.Text, context);
        new ToastContentBuilder()
            .AddText(text)
            .Show();
        return context;
    }
}
