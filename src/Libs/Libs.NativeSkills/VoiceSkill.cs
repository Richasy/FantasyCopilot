// Copyright (c) Fantasy Copilot. All rights reserved.

using System.IO;
using System.Threading.Tasks;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Workspace.Steps;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Windows.Storage;

namespace FantasyCopilot.Libs.NativeSkills;

/// <summary>
/// Voice skill.
/// </summary>
public sealed class VoiceSkill
{
    private readonly IVoiceService _voiceService;
    private readonly WorkflowContext _workflowContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="VoiceSkill"/> class.
    /// </summary>
    public VoiceSkill()
    {
        _voiceService = Locator.Current.GetService<IVoiceService>();
        _workflowContext = Locator.Current.GetVariable<WorkflowContext>();
    }

    /// <summary>
    /// Read text.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Audio file path.</returns>
    [SKFunctionName(WorkflowConstants.Voice.ReadTextName)]
    [SKFunction(WorkflowConstants.Voice.ReadTextDescription)]
    [SKFunctionContextParameter(Description = "The text content to be read.", Name = "INPUT")]
    public async Task<string> ReadAsync(SKContext context)
    {
        var parameters = _workflowContext.GetStepParameters<TextToSpeechStep>();
        if (parameters == null)
        {
            context.Fail("Do not have text-to-speech parameters");
            return default;
        }

        var text = context.Result;
        if (string.IsNullOrEmpty(text))
        {
            context.Fail("The text content to be read cannot be empty");
        }

        var stream = await _voiceService.GetSpeechAsync(text, parameters.Voice);
        var folderPath = ApplicationData.Current.TemporaryFolder.Path;
        var filePath = Path.Combine(folderPath, WorkflowConstants.Voice.ReadTextName + _workflowContext.CurrentStepIndex + ".wav");
        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream);
        return filePath;
    }
}
