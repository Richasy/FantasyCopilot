// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.AI.Translation.Text;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.Constants;
using static FantasyCopilot.AppServices.Utils.BasicUtils;

namespace FantasyCopilot.AppServices.Utils;

internal sealed class AzureTranslateUtils
{
    private readonly TextTranslationClient _translationClient;

    public AzureTranslateUtils()
    {
        var translateKey = ReadLocalSetting(SettingNames.AzureTranslateKey, string.Empty);
        var hasRegion = IsSettingKeyExist(SettingNames.AzureTranslateRegion);

        if (!string.IsNullOrEmpty(translateKey) && hasRegion)
        {
            var region = ReadLocalSetting(SettingNames.AzureTranslateRegion, string.Empty);
            _translationClient = new TextTranslationClient(new AzureKeyCredential(translateKey), region);
        }
        else
        {
            throw new System.Exception("Invalid Azure Translate config");
        }
    }

    internal async Task<IEnumerable<LocaleInfo>> GetSupportLanguagesAsync()
    {
        var data = await GetDataFromFileAsync<List<string>>(AppConstants.AzureTranslateLanguagesFileName, default);
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

    internal async Task<string> TranslateTextAsync(string text, string targetLanguage, TextType type, CancellationToken cancellationToken)
    {
        var languages = await GetSupportLanguagesAsync();
        var targetLanguageInfo = languages.FirstOrDefault(p => p.Id.StartsWith(targetLanguage, System.StringComparison.InvariantCultureIgnoreCase))
            ?? throw new System.Exception("Invalid target language");

        var response = await _translationClient.TranslateAsync(new List<string> { targetLanguageInfo.Id }, new List<string> { text }, default, textType: type, cancellationToken: cancellationToken);
        var translations = response.Value;
        var content = translations.FirstOrDefault()?.Translations?.FirstOrDefault().Text;
        return content ?? string.Empty;
    }

    private async Task<List<string>> GetLanguagesFromOnlineAsync()
    {
        var languages = await _translationClient.GetLanguagesAsync();
        var translation = languages?.Value?.Translation;
        var data = new List<string>();
        if (translation != null)
        {
            foreach (var item in translation)
            {
                data.Add(item.Key);
            }
        }

        return data;
    }
}
