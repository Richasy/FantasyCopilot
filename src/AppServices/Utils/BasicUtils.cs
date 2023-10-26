// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using RichasyAssistant.Models.Constants;
using Windows.Storage;

namespace RichasyAssistant.AppServices.Utils;

internal static class BasicUtils
{
    internal static T ReadLocalSetting<T>(SettingNames settingName, T defaultValue)
    {
        var settingContainer = ApplicationData.Current.LocalSettings;

        if (IsSettingKeyExist(settingName))
        {
            if (defaultValue is Enum)
            {
                var tempValue = settingContainer.Values[settingName.ToString()].ToString();
                Enum.TryParse(typeof(T), tempValue, out var result);
                return (T)result;
            }
            else
            {
                return (T)settingContainer.Values[settingName.ToString()];
            }
        }
        else
        {
            return defaultValue;
        }
    }

    internal static async Task<T> GetDataFromFileAsync<T>(string fileName, T defaultData)
    {
        var filePath = Path.Combine(GetLocalFolderPath(), fileName);
        if (!File.Exists(filePath))
        {
            return defaultData;
        }

        var content = await File.ReadAllTextAsync(filePath);
        return string.IsNullOrEmpty(content) ? defaultData : JsonSerializer.Deserialize<T>(content);
    }

    internal static string GetLocalFolderPath()
        => ApplicationData.Current.LocalFolder.Path;

    internal static bool IsSettingKeyExist(SettingNames settingName)
        => ApplicationData.Current.LocalSettings.Values.ContainsKey(settingName.ToString());
}
