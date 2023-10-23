// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Toolkits.Interfaces;

/// <summary>
/// Interface of settings toolkit.
/// </summary>
public interface ISettingsToolkit
{
    /// <summary>
    /// Read the local setting and convert it to the corresponding value.
    /// Provide a default value when the setting does not exist.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="settingName">Setting key.</param>
    /// <param name="defaultValue">Default value when the setting does not exist.</param>
    /// <returns>Saved data.</returns>
    T ReadLocalSetting<T>(SettingNames settingName, T defaultValue);

    /// <summary>
    /// Write local setting.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="settingName">Setting key.</param>
    /// <param name="value">Data.</param>
    void WriteLocalSetting<T>(SettingNames settingName, T value);

    /// <summary>
    /// Delete local setting.
    /// </summary>
    /// <param name="settingName">Setting key.</param>
    void DeleteLocalSetting(SettingNames settingName);

    /// <summary>
    /// Does the setting name exist.
    /// </summary>
    /// <param name="settingName">Setting key.</param>
    /// <returns><c>True</c> if exist.</returns>
    bool IsSettingKeyExist(SettingNames settingName);

    /// <summary>
    /// Securely store text content.
    /// </summary>
    /// <param name="settingName">Key.</param>
    /// <param name="value">The value stored.</param>
    void SaveSecureString(SettingNames settingName, string value);

    /// <summary>
    /// Retrieve text that was stored safely.
    /// </summary>
    /// <param name="settingName">Key.</param>
    /// <returns>Stored text or empty.</returns>
    Task<string> RetrieveSecureStringAsync(SettingNames settingName);

    /// <summary>
    /// Migrate secure string to local setting.
    /// </summary>
    /// <param name="settingName">Key.</param>
    void MigrateSecureStringToLocalSetting(SettingNames settingName);
}
