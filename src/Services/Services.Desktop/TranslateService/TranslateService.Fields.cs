// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;

namespace FantasyCopilot.Services;

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
