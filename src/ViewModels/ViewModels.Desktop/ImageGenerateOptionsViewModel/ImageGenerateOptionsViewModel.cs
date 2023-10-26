// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.Image;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Image generate options view model.
/// </summary>
public sealed partial class ImageGenerateOptionsViewModel : ViewModelBase, IImageGenerateOptionsViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageGenerateOptionsViewModel"/> class.
    /// </summary>
    public ImageGenerateOptionsViewModel(
        ISettingsToolkit settingsToolkit)
        => _settingsToolkit = settingsToolkit;

    /// <inheritdoc/>
    public GenerateOptions GetOptions()
        => new GenerateOptions
        {
            ClipSkip = ClipSkip,
            Model = Model,
            Sampler = Sampler,
            Width = Width,
            Height = Height,
            CfgScale = CfgScale,
            Seed = System.Convert.ToInt64(Seed),
            Steps = Steps,
        };

    /// <inheritdoc/>
    public void Initialize(GenerateOptions options = null)
    {
        if (options == null)
        {
            ClipSkip = _settingsToolkit.ReadLocalSetting(SettingNames.DefaultSDClipSkip, 2);
            Width = _settingsToolkit.ReadLocalSetting(SettingNames.DefaultSDWidth, 512);
            Height = _settingsToolkit.ReadLocalSetting(SettingNames.DefaultSDHeight, 512);
            CfgScale = _settingsToolkit.ReadLocalSetting(SettingNames.DefaultSDCfgScale, 7d);
            Seed = System.Convert.ToDouble(_settingsToolkit.ReadLocalSetting(SettingNames.DefaultSDSeed, -1L));
            Steps = _settingsToolkit.ReadLocalSetting(SettingNames.DefaultSDSteps, 20);
        }
        else
        {
            ClipSkip = options.ClipSkip;
            Width = options.Width;
            Height = options.Height;
            CfgScale = options.CfgScale;
            Seed = System.Convert.ToDouble(options.Seed);
            Steps = options.Steps;
            Model = options.Model;
            Sampler = options.Sampler;
        }
    }
}
