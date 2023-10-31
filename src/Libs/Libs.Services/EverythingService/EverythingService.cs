// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using System.Threading.Tasks;
using RichasyAssistant.Libs.Everything.Interfaces;
using RichasyAssistant.Models.App.Files;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Everything storage service.
/// </summary>
public sealed partial class EverythingService : ServiceBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EverythingService"/> class.
    /// </summary>
    private EverythingService()
    {
    }

    /// <summary>
    /// Does the current user have a valid config.
    /// </summary>
    public bool HasValidConfig => _hasValidConfig;

    /// <summary>
    /// Reload config.
    /// </summary>
    public void ReloadConfig()
    {
        _client = default;
        CheckConfig();
    }

    /// <summary>
    /// Gets the matching files.
    /// </summary>
    /// <param name="type">Search type.</param>
    /// <param name="query">Query text.</param>
    /// <returns>File list.</returns>
    public async Task<List<FileItem>> SearchAudiosAsync(AudioSearchType type, string query)
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

    /// <summary>
    /// Gets the matching files.
    /// </summary>
    /// <param name="type">Search type.</param>
    /// <param name="query">Query text.</param>
    /// <returns>File list.</returns>
    public async Task<List<FileItem>> SearchFilesAsync(FileSearchType type, string query)
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

    /// <summary>
    /// Gets the matching images.
    /// </summary>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    /// <param name="imageOrientation">Image orientation.</param>
    /// <param name="imageColorDepth">Image bit depth.</param>
    /// <returns>File list.</returns>
    public async Task<List<FileItem>> SearchImagesAsync(int width, int height, ImageOrientation imageOrientation, ImageColorDepth imageColorDepth)
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

    /// <summary>
    /// Gets the matching entries.
    /// </summary>
    /// <param name="keyword">Keyword.</param>
    /// <returns>File list.</returns>
    public async Task<List<FileItem>> SearchItemsAsync(string keyword)
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
