// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;

namespace FantasyCopilot.Services;

/// <summary>
/// Baidu translate service.
/// </summary>
public sealed partial class BaiduTranslateService
{
    private const string TranslateApi = "http://api.fanyi.baidu.com/api/trans/vip/translate";
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly HttpClient _httpClient;
    private string _salt;
    private string _appId;
    private string _appKey;

    /// <inheritdoc/>
    public bool HasValidConfig { get; set; }

    private void CheckConfig()
    {
        _appId = _settingsToolkit.RetrieveSecureString(SettingNames.BaiduTranslateAppId);
        _appKey = _settingsToolkit.RetrieveSecureString(SettingNames.BaiduTranslateAppKey);

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
