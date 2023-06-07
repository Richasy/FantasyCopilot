// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;

namespace FantasyCopilot.Services;

/// <summary>
/// Azure translate service.
/// </summary>
public sealed partial class AzureTranslateService : ITranslateService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureVoiceService"/> class.
    /// </summary>
    public AzureTranslateService(
        ISettingsToolkit settingsToolkit,
        IFileToolkit fileToolkit)
    {
        _settingsToolkit = settingsToolkit;
        _fileToolkit = fileToolkit;
    }

    /// <inheritdoc/>
    public bool HasValidConfig
    {
        get
        {
            CheckConfig();
            return _hasValidConfig;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<LocaleInfo>> GetSupportLanguagesAsync()
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

    /// <inheritdoc/>
    public void ReloadConfig()
    {
        _translationClient = default;
        CheckConfig();
    }

    /// <inheritdoc/>
    public async Task<string> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage, CancellationToken cancellationToken)
    {
        var response = await _translationClient.TranslateAsync(targetLanguage, text, sourceLanguage, cancellationToken);
        var translations = response.Value;
        var content = translations.FirstOrDefault()?.Translations?.FirstOrDefault().Text;
        return content ?? string.Empty;
    }
}
