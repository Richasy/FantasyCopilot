// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Workspace.Steps;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Services.Interfaces;
using Windows.Storage;

namespace RichasyAssistant.Libs.NativeSkills;

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
    [SKName(WorkflowConstants.Voice.ReadTextName)]
    [Description(WorkflowConstants.Voice.ReadTextDescription)]
    [SKFunction]
    public async Task<string> ReadAsync(SKContext context)
    {
        var parameters = _workflowContext.GetStepParameters<TextToSpeechStep>()
            ?? throw new SKException("Do not have text-to-speech parameters");

        var text = context.Result;
        if (string.IsNullOrEmpty(text))
        {
            throw new SKException("The text content to be read cannot be empty");
        }

        var stream = await _voiceService.GetSpeechAsync(text, parameters.Voice);
        var folderPath = ApplicationData.Current.TemporaryFolder.Path;
        var filePath = Path.Combine(folderPath, WorkflowConstants.Voice.ReadTextName + _workflowContext.CurrentStepIndex + ".wav");
        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream);
        return filePath;
    }
}
