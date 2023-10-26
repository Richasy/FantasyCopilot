// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.Constants;
using RichasyAssistant.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.Services;

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
