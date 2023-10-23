// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App;

namespace RichasyAssistant.Services.Interfaces;

/// <summary>
/// Interface definition for translate service.
/// </summary>
public interface ITranslateService : IConfigServiceBase
{
    /// <summary>
    /// Get a list of supported languages.
    /// </summary>
    /// <returns>Language list.</returns>
    Task<IEnumerable<LocaleInfo>> GetSupportLanguagesAsync();

    /// <summary>
    /// Translate text.
    /// </summary>
    /// <param name="text">Input content.</param>
    /// <param name="sourceLanguage">Source langauge.</param>
    /// <param name="targetLanguage">Target language.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Translated content.</returns>
    Task<string> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage, CancellationToken cancellationToken);
}
