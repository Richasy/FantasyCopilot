// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.Generic;
using System.Threading.Tasks;
using FantasyCopilot.Libs.Everything.Interfaces;
using FantasyCopilot.Models.App.Files;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FantasyCopilot.Services;

/// <summary>
/// Everything storage service.
/// </summary>
public sealed partial class EverythingStorageService : IStorageService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EverythingStorageService"/> class.
    /// </summary>
    public EverythingStorageService(ILogger<EverythingStorageService> logger)
        => _logger = logger;

    /// <inheritdoc/>
    public bool HasValidConfig => _hasValidConfig;

    /// <inheritdoc/>
    public void ReloadConfig()
    {
        _client = default;
        CheckConfig();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<FileItem>> SearchAudiosAsync(AudioSearchType type, string query)
    {
        if (_client == null)
        {
            return default;
        }

        var items = new List<FileItem>();
        await Task.Run(() =>
        {
            var queryResult = type switch
            {
                AudioSearchType.Track => SearchAudiosWithTrack(query),
                AudioSearchType.Album => _client.Search().Music.Album(query),
                AudioSearchType.Artist => _client.Search().Music.Artist(query),
                AudioSearchType.Genre => _client.Search().Music.Genre(query),
                AudioSearchType.Title => _client.Search().Music.Title(query),
                AudioSearchType.Comment => _client.Search().Music.Comment(query),
                _ => throw new System.NotImplementedException(),
            };
            LoadItems(queryResult, ref items);
        });

        _client.Reset();
        return items;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<FileItem>> SearchFilesAsync(FileSearchType type, string query)
    {
        if (_client == null)
        {
            return default;
        }

        var items = new List<FileItem>();
        await Task.Run(() =>
        {
            var queryResult = type switch
            {
                FileSearchType.Children => SearchFilesWithChildren(query),
                FileSearchType.Audio => _client.Search().File.Audio(query),
                FileSearchType.Zip => _client.Search().File.Zip(query),
                FileSearchType.Video => _client.Search().File.Video(query),
                FileSearchType.Picture => _client.Search().File.Picture(query),
                FileSearchType.Exe => _client.Search().File.Exe(query),
                FileSearchType.Document => _client.Search().File.Document(query),
                FileSearchType.Duplicates => _client.Search().File.Duplicates(query),
                _ => throw new System.NotImplementedException(),
            };
            LoadItems(queryResult, ref items);
        });

        _client.Reset();
        return items;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<FileItem>> SearchImagesAsync(int width, int height, ImageOrientation imageOrientation, ImageColorDepth imageColorDepth)
    {
        if (_client == null)
        {
            return default;
        }

        var items = new List<FileItem>();
        await Task.Run(() =>
        {
            var query = _client.Search().Image;
            var hasProperty = false;
            if (width > 0)
            {
                query = query.Width(width);
                hasProperty = true;
            }

            if (height > 0)
            {
                query = hasProperty ? query.And.Image.Height(height) : query.Height(height);
                hasProperty = true;
            }

            if (imageOrientation != ImageOrientation.All)
            {
                query = hasProperty
                    ? imageOrientation == ImageOrientation.Portrait ? query.And.Image.Portrait() : query.And.Image.Landscape()
                    : imageOrientation == ImageOrientation.Portrait ? query.Portrait() : query.Landscape();
                hasProperty = true;
            }

            if (imageColorDepth != ImageColorDepth.All)
            {
                var bpp = (Bpp)((int)imageColorDepth - 1);
                query = hasProperty ? query.And.Image.BitDepth(bpp) : query.BitDepth(bpp);
            }

            LoadItems(query, ref items);
        });

        _client.Reset();
        return items;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<FileItem>> SearchItemsAsync(string keyword)
    {
        if (_client == null)
        {
            return default;
        }

        var items = new List<FileItem>();
        await Task.Run(() =>
        {
            var queryResults = _client.Search()
                .Name
                .Contains(keyword);
            LoadItems(queryResults, ref items);
        });

        _client.Reset();
        return items;
    }
}
