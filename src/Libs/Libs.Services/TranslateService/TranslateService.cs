// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RichasyAssistant.Libs.Services.Interfaces;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Translate service.
/// </summary>
public sealed partial class TranslateService : ITranslateService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateService"/> class.
    /// </summary>
    private TranslateService()
    {
    }

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static TranslateService Instance { get; } = new TranslateService();

    /// <summary>
    /// Get a list of supported languages.
    /// </summary>
    /// <returns>Language list.</returns>
    public Task<List<LocaleInfo>> GetSupportLanguagesAsync()
        => _currentService?.GetSupportLanguagesAsync() ?? Task.FromResult<List<LocaleInfo>>(default);

    /// <inheritdoc/>
    public void ReloadConfig()
    {
        var translateService = _settingsToolkit.ReadLocalSetting(SettingNames.TranslateSource, TranslateSource.Azure);
        if (_currentTranslateSource != translateService || _currentService == null)
        {
            _currentTranslateSource = translateService;
            _currentService = translateService switch
            {
                TranslateSource.Azure => AzureTranslateService.Instance,
                TranslateSource.Baidu => BaiduTranslateService.Instance,
                _ => default
            };
        }

        _currentService.ReloadConfig();
    }

    /// <inheritdoc/>
    public Task<string> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage, CancellationToken cancellationToken)
        => _currentService?.TranslateTextAsync(text, sourceLanguage, targetLanguage, cancellationToken) ?? Task.FromResult(string.Empty);
}
