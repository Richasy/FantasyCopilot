// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Toolkits.Interfaces;

/// <summary>
/// Interface of file toolkit.
/// </summary>
public interface IFileToolkit
{
    /// <summary>
    /// Write content to the file.
    /// </summary>
    /// <param name="content">Text content.</param>
    /// <param name="fileName">Filename (can be a path).</param>
    /// <param name="isOverwrite">Do we need to overwrite files with the same name.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task WriteContentAsync(string content, string fileName, bool isOverwrite = true);

    /// <summary>
    /// Get data from file.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    /// <param name="fileName">Filename (can be a path).</param>
    /// <param name="defaultData">Default data if file does not exist.</param>
    /// <returns>Data.</returns>
    Task<T> GetDataFromFileAsync<T>(string fileName, T defaultData);

    /// <summary>
    /// Delete specified file.
    /// </summary>
    /// <param name="fileName">Filename (can be a path).</param>
    /// <returns><see cref="Task"/>.</returns>
    Task DeleteFileAsync(string fileName);

    /// <summary>
    /// Open the file save picker and get the saved file instance.
    /// </summary>
    /// <param name="suggestedFileName">Suggested file name.</param>
    /// <param name="windowInstance">Current window instance.</param>
    /// <returns>File instance.</returns>
    Task<object> SaveFileAsync(string suggestedFileName, object windowInstance);

    /// <summary>
    /// Open the file picker and get the selected file instance.
    /// </summary>
    /// <param name="extension">File extension filter.</param>
    /// <param name="windowInstance">Current window instance.</param>
    /// <returns>File instance.</returns>
    Task<object> PickFileAsync(string extension, object windowInstance);

    /// <summary>
    /// Open the folder picker and get the selected folder instance.
    /// </summary>
    /// <param name="windowInstance">Current window instance.</param>
    /// <returns>Folder instance.</returns>
    Task<object> PickFolderAsync(object windowInstance);
}
