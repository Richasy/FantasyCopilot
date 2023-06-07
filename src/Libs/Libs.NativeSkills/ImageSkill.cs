// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.IO;
using System.Threading.Tasks;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Workspace.Steps;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.TemplateEngine;
using Windows.Storage;

namespace FantasyCopilot.Libs.NativeSkills;

/// <summary>
/// Image skill.
/// </summary>
public sealed class ImageSkill
{
    private readonly IImageService _imageService;
    private readonly ICacheToolkit _cacheToolkit;
    private readonly WorkflowContext _workflowContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageSkill"/> class.
    /// </summary>
    public ImageSkill()
    {
        _imageService = Locator.Current.GetService<IImageService>();
        _cacheToolkit = Locator.Current.GetService<ICacheToolkit>();
        _workflowContext = Locator.Current.GetVariable<WorkflowContext>();
    }

    /// <summary>
    /// Draw image.
    /// </summary>
    /// <param name="context">Current context.</param>
    /// <returns>Image path.</returns>
    [SKFunctionName(WorkflowConstants.Image.DrawName)]
    [SKFunction(WorkflowConstants.Image.DrawDescription)]
    public async Task<string> DrawAsync(SKContext context)
    {
        var parameters = _workflowContext.GetStepParameters<ImageStep>();
        if (parameters == null)
        {
            context.Fail("Do not have text-to-image parameters");
            return default;
        }

        var config = await _cacheToolkit.GetImageSkillByIdAsync(parameters.Id);
        if (config == null)
        {
            context.Fail("Image skill config not found.");
            return default;
        }

        var templateEngine = new PromptTemplateEngine();
        var prompt = await templateEngine.RenderAsync(config.Prompt, context);
        var negativePrompt = await templateEngine.RenderAsync(config.NegativePrompt, context);
        var (imageData, imageDesc) = await _imageService.GenerateImageAsync(prompt, negativePrompt, config, context.CancellationToken);
        var descKey = string.Format(WorkflowConstants.Txt2ImgResultKey, _workflowContext.CurrentStepIndex);
        context.Variables.Set(descKey, imageDesc);
        var tempFilePath = Path.Combine(ApplicationData.Current.TemporaryFolder.Path, Guid.NewGuid().ToString("N") + ".jpeg");
        await File.WriteAllBytesAsync(tempFilePath, imageData, context.CancellationToken);
        return tempFilePath;
    }
}
