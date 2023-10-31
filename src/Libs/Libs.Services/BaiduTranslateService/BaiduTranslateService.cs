// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Libs.Services.Interfaces;
using RichasyAssistant.Models.App;
using RichasyAssistant.Toolkits.Interfaces;
using Windows.Storage;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Baidu translate service.
/// </summary>
internal sealed partial class BaiduTranslateService : ITranslateService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaiduTranslateService"/> class.
    /// </summary>
    public BaiduTranslateService()
    {
        _settingsToolkit = Locator.Current.GetService<ISettingsToolkit>();
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Get a list of supported languages.
    /// </summary>
    /// <returns>Language list.</returns>
    public async Task<List<LocaleInfo>> GetSupportLanguagesAsync()
    {
        var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/BaiduTranslateSupportLanguages.json"));
        var json = await FileIO.ReadTextAsync(file);
        var data = JsonSerializer.Deserialize<List<string>>(json);
        return data.Select(p => new LocaleInfo(new CultureInfo(p))).ToList();
    }

    /// <summary>
    /// Reload config.
    /// </summary>
    public void ReloadConfig()
    {
        CheckConfig();
        _salt = new Random().Next(100000).ToString();
    }

    /// <summary>
    /// Translate text.
    /// </summary>
    /// <param name="text">Input content.</param>
    /// <param name="sourceLanguage">Source langauge.</param>
    /// <param name="targetLanguage">Target language.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Translated content.</returns>
    public async Task<string> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(sourceLanguage))
        {
            sourceLanguage = "auto";
        }

        text = text.Replace("\r", "\n").Replace("\n\n", "\n");
        var textBuilder = new StringBuilder();
        var resultBuilder = new StringBuilder();
        foreach (var item in text.Split('\n'))
        {
            if (textBuilder.Length + item.Length > MaxTextLength)
            {
                // Due to the API request rate limit,
                // a translation is requested at most once per second,
                // so additional blocking is required here
                if (resultBuilder.Length > 0)
                {
                    await Task.Delay(1000, cancellationToken);
                }

                var tempResult = await TranslateInternalAsync(textBuilder.ToString(), sourceLanguage, targetLanguage, cancellationToken);
                resultBuilder.AppendLine(tempResult);
                textBuilder.Clear();
            }

            textBuilder.AppendLine(item);
        }

        if (textBuilder.Length > 0)
        {
            if (resultBuilder.Length > 0)
            {
                await Task.Delay(1000, cancellationToken);
            }

            var tempResult = await TranslateInternalAsync(textBuilder.ToString(), sourceLanguage, targetLanguage, cancellationToken);
            resultBuilder.AppendLine(tempResult);
        }

        return resultBuilder.ToString();
    }

    private async Task<string> TranslateInternalAsync(string text, string sourceLanguage, string targetLanguage, CancellationToken cancellationToken)
    {
        text = text.Replace("\r", "\n").Replace("\n\n", "\n");
        var dict = new Dictionary<string, string>
        {
            { "q", text },
            { "from", sourceLanguage },
            { "to", targetLanguage },
            { "appid", _appId },
            { "salt", _salt },
            { "sign", GenerateSign(text) },
        };

        var content = new FormUrlEncodedContent(dict);
        var request = new HttpRequestMessage(HttpMethod.Post, TranslateApi);
        request.Content = content;
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        var jele = JsonSerializer.Deserialize<JsonElement>(result);
        if (jele.TryGetProperty("error_msg", out var errorEle))
        {
            throw new Exception(errorEle.GetRawText());
        }
        else
        {
            var sb = new StringBuilder();
            var hasResult = jele.TryGetProperty("trans_result", out var transResultEle);
            if (!hasResult)
            {
                throw new Exception("No result.");
            }

            foreach (var item in transResultEle.EnumerateArray())
            {
                if (item.TryGetProperty("dst", out var dstEle))
                {
                    sb.AppendLine(dstEle.ToString());
                }
            }

            return sb.ToString().Trim();
        }
    }
}
