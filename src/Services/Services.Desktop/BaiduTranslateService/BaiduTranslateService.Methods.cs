// Copyright (c) Richasy Assistant. All rights reserved.

using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.Services;

/// <summary>
/// Baidu translate service.
/// </summary>
public sealed partial class BaiduTranslateService
{
    private const string TranslateApi = "http://api.fanyi.baidu.com/api/trans/vip/translate";
    private const int MaxTextLength = 1100;
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly HttpClient _httpClient;
    private string _salt;
    private string _appId;
    private string _appKey;

    /// <inheritdoc/>
    public bool HasValidConfig { get; set; }

    private void CheckConfig()
    {
        _appId = _settingsToolkit.ReadLocalSetting(SettingNames.BaiduTranslateAppId, string.Empty);
        _appKey = _settingsToolkit.ReadLocalSetting(SettingNames.BaiduTranslateAppKey, string.Empty);

        HasValidConfig = !string.IsNullOrEmpty(_appId) && !string.IsNullOrEmpty(_appKey);
    }

    private string GenerateSign(string query)
    {
        var byteOld = Encoding.UTF8.GetBytes(_appId + query + _salt + _appKey);
        var byteNew = MD5.HashData(byteOld);
        var sb = new StringBuilder();
        foreach (var b in byteNew)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}
