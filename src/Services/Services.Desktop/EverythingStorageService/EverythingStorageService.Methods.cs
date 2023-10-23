// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using NeoSmart.PrettySize;
using RichasyAssistant.Libs.Everything.Core;
using RichasyAssistant.Libs.Everything.Interfaces;
using RichasyAssistant.Models.App.Files;

namespace RichasyAssistant.Services;

/// <summary>
/// Everything storage service.
/// </summary>
public sealed partial class EverythingStorageService
{
    private readonly ILogger<EverythingStorageService> _logger;
    private bool _hasValidConfig;
    private string _appPath;
    private Everything _client;

    private void LoadItems(IQueryable queryResults, ref List<FileItem> items)
    {
        if (queryResults == null)
        {
            _logger.LogInformation("No file search results found");
            return;
        }

        foreach (var item in queryResults)
        {
            var fileItem = new FileItem
            {
                Name = item.FileName,
                Path = item.FullPath,
                CreatedTime = item.Created,
                LastModifiedTime = item.Modified,
                ByteLength = item.Size,
            };

            if (item.Size > 0)
            {
                fileItem.FileSize = PrettySize.Bytes(item.Size).Format(UnitBase.Base10, UnitStyle.Abbreviated);
            }

            fileItem.Extension = Directory.Exists(item.FullPath)
                ? "_"
                : string.IsNullOrEmpty(Path.GetExtension(item.FullPath))
                    ? "__"
                    : Path.GetExtension(item.FullPath).TrimStart('.');

            items.Add(fileItem);
        }

        _logger.LogInformation($"{items.Count} entries have been searched");
    }

    private void CheckConfig()
    {
        try
        {
            var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Everything")?.GetValue("DisplayIcon");
            _appPath = key is string location ? location : string.Empty;
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, $"Attempt to get Everything install location failed");
        }

        if (_client == default)
        {
            _client = new Everything(_appPath);
        }

        try
        {
            var major = _client.GetMajorVersion();
            _hasValidConfig = major > 0;
        }
        catch (Exception)
        {
            _hasValidConfig = false;
        }
    }

    private IFileQueryable SearchFilesWithChildren(string folderPath)
    {
        try
        {
            Path.GetFullPath(folderPath);
        }
        catch (Exception)
        {
            return default;
        }

        var isExistPath = Path.Exists(folderPath);
        return !isExistPath ? default : _client.Search().File.Parent(folderPath);
    }

    private IMusicQueryable SearchAudiosWithTrack(string track)
    {
        var parseResult = int.TryParse(track, out var trackNum);
        return !parseResult || trackNum < 0 ? default : _client.Search().Music.Track(trackNum);
    }
}
