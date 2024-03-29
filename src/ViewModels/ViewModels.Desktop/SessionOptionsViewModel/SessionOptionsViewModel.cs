﻿// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.Gpt;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Session options view model.
/// </summary>
public sealed partial class SessionOptionsViewModel : ViewModelBase, ISessionOptionsViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SessionOptionsViewModel"/> class.
    /// </summary>
    public SessionOptionsViewModel(ISettingsToolkit settingsToolkit)
        => _settingsToolkit = settingsToolkit;

    /// <inheritdoc/>
    public void Initialize(SessionOptions options = default)
    {
        IsStreamOutputEnabled = true;
        if (options == null)
        {
            Temperature = _settingsToolkit.ReadLocalSetting(SettingNames.DefaultTemperature, 0.4);
            MaxResponseTokens = _settingsToolkit.ReadLocalSetting(SettingNames.DefaultMaxResponseTokens, AppConstants.DefaultMaxResponseTokens);
            TopP = _settingsToolkit.ReadLocalSetting(SettingNames.DefaultTopP, 0d);
            FrequencyPenalty = _settingsToolkit.ReadLocalSetting(SettingNames.DefaultFrequencyPenalty, 0d);
            PresencePenalty = _settingsToolkit.ReadLocalSetting(SettingNames.DefaultPresencePenalty, 0d);
            UseStreamOutput = true;
            AutoRemoveEarlierMessage = false;
        }
        else
        {
            Temperature = options.Temperature;
            MaxResponseTokens = options.MaxResponseTokens;
            TopP = options.TopP;
            FrequencyPenalty = options.FrequencyPenalty;
            PresencePenalty = options.PresencePenalty;
            UseStreamOutput = options.UseStreamOutput;
            AutoRemoveEarlierMessage = options.AutoRemoveEarlierMessage;
        }
    }

    /// <inheritdoc/>
    public SessionOptions GetOptions()
        => new SessionOptions
        {
            Temperature = Temperature,
            MaxResponseTokens = MaxResponseTokens,
            TopP = TopP,
            FrequencyPenalty = FrequencyPenalty,
            PresencePenalty = PresencePenalty,
            UseStreamOutput = IsStreamOutputEnabled && UseStreamOutput,
            AutoRemoveEarlierMessage = AutoRemoveEarlierMessage,
        };
}
