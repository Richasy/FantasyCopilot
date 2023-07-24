// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Reflection;
using System.Threading.Tasks;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using Windows.Security.Credentials;
using Windows.Storage;

namespace FantasyCopilot.Toolkits;

/// <summary>
/// Settings toolkit.
/// </summary>
public sealed class SettingsToolkit : ISettingsToolkit
{
    /// <inheritdoc/>
    public T ReadLocalSetting<T>(SettingNames settingName, T defaultValue)
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

    /// <inheritdoc/>
    public void WriteLocalSetting<T>(SettingNames settingName, T value)
    {
        var settingContainer = ApplicationData.Current.LocalSettings;
        settingContainer.Values[settingName.ToString()] = value is Enum ? value.ToString() : value;
    }

    /// <inheritdoc/>
    public void DeleteLocalSetting(SettingNames settingName)
    {
        var settingContainer = ApplicationData.Current.LocalSettings;

        if (IsSettingKeyExist(settingName))
        {
            settingContainer.Values.Remove(settingName.ToString());
        }
    }

    /// <inheritdoc/>
    public bool IsSettingKeyExist(SettingNames settingName)
        => ApplicationData.Current.LocalSettings.Values.ContainsKey(settingName.ToString());

    /// <inheritdoc/>
    public void SaveSecureString(SettingNames settingName, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        var credential = new PasswordCredential(Assembly.GetAssembly(GetType()).FullName, settingName.ToString(), value);
        new PasswordVault().Add(credential);
    }

    /// <inheritdoc/>
    public async Task<string> RetrieveSecureStringAsync(SettingNames settingName)
    {
        return await Task.Run(() =>
        {
            try
            {
                var credential = new PasswordVault().Retrieve(Assembly.GetAssembly(GetType()).FullName, settingName.ToString());
                credential.RetrievePassword();
                return credential.Password;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        });
    }
}
