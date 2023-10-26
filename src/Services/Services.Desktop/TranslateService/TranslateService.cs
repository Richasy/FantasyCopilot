// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.Services;

/// <summary>
/// Translate service.
/// </summary>
public sealed partial class TranslateService : ITranslateService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateService"/> class.
    /// </summary>
    public TranslateService(ISettingsToolkit settingsToolkit)
        => _settingsToolkit = settingsToolkit;

    /// <inheritdoc/>
    public Task<IEnumerable<LocaleInfo>> GetSupportLanguagesAsync()
        => _currentService?.GetSupportLanguagesAsync() ?? Task.FromResult<IEnumerable<LocaleInfo>>(default);

    /// <inheritdoc/>
    public void ReloadConfig()
    {
        var translateService = _settingsToolkit.ReadLocalSetting(SettingNames.TranslateSource, TranslateSource.Azure);
        if (_currentTranslateSource != translateService || _currentService == null)
        {
            _currentTranslateSource = translateService;
            _currentService = translateService switch
            {
                TranslateSource.Azure => new AzureTranslateService(),
                TranslateSource.Baidu => new BaiduTranslateService(),
                _ => default
            };
        }

        _currentService.ReloadConfig();
    }

    /// <inheritdoc/>
    public Task<string> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage, CancellationToken cancellationToken)
        => _currentService?.TranslateTextAsync(text, sourceLanguage, targetLanguage, cancellationToken) ?? Task.FromResult(string.Empty);
}
