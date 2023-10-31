// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Libs.Services.Interfaces;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Azure translate service.
/// </summary>
internal sealed partial class AzureTranslateService : ServiceBase, ITranslateService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureVoiceService"/> class.
    /// </summary>
    private AzureTranslateService()
    {
        _settingsToolkit = Locator.Current.GetService<ISettingsToolkit>();
        _fileToolkit = Locator.Current.GetService<IFileToolkit>();
    }

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static AzureTranslateService Instance { get; } = new AzureTranslateService();

    /// <summary>
    /// Get support languages from online.
    /// </summary>
    /// <returns>Language list.</returns>
    public async Task<List<LocaleInfo>> GetSupportLanguagesAsync()
    {
        var data = await _fileToolkit.GetDataFromFileAsync<List<string>>(AppConstants.AzureTranslateLanguagesFileName, default);
        if (data == null)
        {
            data = await GetLanguagesFromOnlineAsync();
            if (data == null)
            {
                return default;
            }
        }

        return data.Select(p => new LocaleInfo(new CultureInfo(p))).ToList();
    }

    /// <summary>
    /// Reload config.
    /// </summary>
    public void ReloadConfig()
    {
        _translationClient = default;
        CheckConfig();
    }

    /// <summary>
    /// Translate text.
    /// </summary>
    /// <param name="text">Source content.</param>
    /// <param name="sourceLanguage">Source language.</param>
    /// <param name="targetLanguage">Target language.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Result.</returns>
    public async Task<string> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage, CancellationToken cancellationToken)
    {
        var response = await _translationClient.TranslateAsync(targetLanguage, text, sourceLanguage, cancellationToken);
        var translations = response.Value;
        var content = translations.FirstOrDefault()?.Translations?.FirstOrDefault().Text;
        return content ?? string.Empty;
    }
}
