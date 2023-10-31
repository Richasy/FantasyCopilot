// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Libs.Services.Interfaces;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Translate service.
/// </summary>
public sealed partial class TranslateService
{
    private readonly ISettingsToolkit _settingsToolkit;
    private ITranslateService _currentService;
    private TranslateSource _currentTranslateSource;

    /// <inheritdoc/>
    public bool HasValidConfig => _currentService?.HasValidConfig ?? false;
}
