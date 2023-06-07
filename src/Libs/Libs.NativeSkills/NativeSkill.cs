// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Threading.Tasks;
using CommunityToolkit.WinUI.Notifications;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Workspace.Steps;
using FantasyCopilot.Models.Constants;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.TemplateEngine;

namespace FantasyCopilot.Libs.NativeSkills;

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
    [SKFunctionName(WorkflowConstants.Native.TextNotificationName)]
    [SKFunction(WorkflowConstants.Native.TextNotificationDescription)]
    public async Task<SKContext> SendNotificationAsync(SKContext context)
    {
        var parameters = _workflowContext.GetStepParameters<TextNotificationStep>();
        if (parameters == null || string.IsNullOrEmpty(parameters.Text))
        {
            context.Fail("No notification parameters found.");
            return context;
        }

        var engine = new PromptTemplateEngine();
        var text = await engine.RenderAsync(parameters.Text, context);
        new ToastContentBuilder()
            .AddText(text)
            .Show();
        return context;
    }
}
