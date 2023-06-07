// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FantasyCopilot.Toolkits.Interfaces;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace FantasyCopilot.Toolkits;

/// <summary>
/// File toolkit.
/// </summary>
public sealed class FileToolkit : IFileToolkit
{
    /// <inheritdoc/>
    public async Task<T> GetDataFromFileAsync<T>(string fileName, T defaultData)
    {
        var filePath = Path.Combine(GetLocalFolderPath(), fileName);
        if (!File.Exists(filePath))
        {
            return defaultData;
        }

        var content = await File.ReadAllTextAsync(filePath);
        return string.IsNullOrEmpty(content) ? defaultData : JsonSerializer.Deserialize<T>(content);
    }

    /// <inheritdoc/>
    public async Task WriteContentAsync(string content, string fileName, bool isOverwrite = true)
    {
        var filePath = Path.Combine(GetLocalFolderPath(), fileName);
        var dir = Path.Combine(GetLocalFolderPath(), Path.GetDirectoryName(filePath));
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        if (File.Exists(filePath) && isOverwrite)
        {
            File.Delete(filePath);
        }

        await File.WriteAllTextAsync(filePath, content, Encoding.UTF8);
    }

    /// <inheritdoc/>
    public async Task DeleteFileAsync(string fileName)
    {
        var filePath = Path.Combine(GetLocalFolderPath(), fileName);
        if (File.Exists(filePath))
        {
            await Task.Run(() =>
            {
                File.Delete(filePath);
            });
        }
    }

    /// <inheritdoc/>
    public async Task<object> SaveFileAsync(string suggestedFileName, object windowInstance)
    {
        var extension = Path.GetExtension(suggestedFileName);
        var fileSavePicker = new FileSavePicker();
        InitializeWithWindow.Initialize(fileSavePicker, WindowNative.GetWindowHandle(windowInstance));
        fileSavePicker.FileTypeChoices.Add(extension.TrimStart('.').ToUpper(), new[] { extension });
        fileSavePicker.DefaultFileExtension = extension;
        fileSavePicker.SuggestedFileName = suggestedFileName;
        fileSavePicker.SuggestedStartLocation = PickerLocationId.Desktop;
        var file = await fileSavePicker.PickSaveFileAsync().AsTask();
        return file;
    }

    /// <inheritdoc/>
    public async Task<object> PickFileAsync(string extension, object windowInstance)
    {
        var picker = new FileOpenPicker();
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(windowInstance));
        picker.FileTypeFilter.Add(extension);
        picker.SuggestedStartLocation = PickerLocationId.Desktop;
        var file = await picker.PickSingleFileAsync().AsTask();
        return file;
    }

    /// <inheritdoc/>
    public async Task<object> PickFolderAsync(object windowInstance)
    {
        var picker = new FolderPicker
        {
            SuggestedStartLocation = PickerLocationId.Desktop,
        };
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(windowInstance));
        var folder = await picker.PickSingleFolderAsync().AsTask();
        return folder;
    }

    private static string GetLocalFolderPath()
        => ApplicationData.Current.LocalFolder.Path;
}
