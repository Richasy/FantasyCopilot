// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Azure.AI.Translation.Text;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Azure translate service.
/// </summary>
internal sealed partial class AzureTranslateService
{
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IFileToolkit _fileToolkit;
    private TextTranslationClient _translationClient;

    /// <summary>
    /// Has valid config.
    /// </summary>
    public bool HasValidConfig { get; set; }

    private void CheckConfig()
    {
        var translateKey = _settingsToolkit.ReadLocalSetting(SettingNames.AzureTranslateKey, string.Empty);
        var hasRegion = _settingsToolkit.IsSettingKeyExist(SettingNames.AzureTranslateRegion);

        HasValidConfig = !string.IsNullOrEmpty(translateKey) && hasRegion;

        if (HasValidConfig && _translationClient == null)
        {
            var region = _settingsToolkit.ReadLocalSetting(SettingNames.AzureTranslateRegion, string.Empty);
            _translationClient = new TextTranslationClient(new AzureKeyCredential(translateKey), region);
        }
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

            await _fileToolkit.WriteContentAsync(JsonSerializer.Serialize(data), AppConstants.AzureTranslateLanguagesFileName);
        }

        return data;
    }
}
