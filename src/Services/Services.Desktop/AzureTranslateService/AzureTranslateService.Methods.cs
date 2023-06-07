// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Azure.AI.Translation.Text;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;

namespace FantasyCopilot.Services;

/// <summary>
/// Azure translate service.
/// </summary>
public sealed partial class AzureTranslateService
{
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IFileToolkit _fileToolkit;
    private bool _hasValidConfig;
    private TextTranslationClient _translationClient;

    private void CheckConfig()
    {
        var translateKey = _settingsToolkit.RetrieveSecureString(SettingNames.AzureTranslateKey);
        var hasRegion = _settingsToolkit.IsSettingKeyExist(SettingNames.AzureTranslateRegion);

        _hasValidConfig = !string.IsNullOrEmpty(translateKey) && hasRegion;

        if (_hasValidConfig && _translationClient == null)
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
