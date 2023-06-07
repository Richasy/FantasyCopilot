// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;

namespace FantasyCopilot.Toolkits;

/// <summary>
/// Cache toolkit.
/// </summary>
public sealed partial class CacheToolkit : ICacheToolkit
{
    private readonly IFileToolkit _fileToolkit;
    private List<SessionMetadata> _sessions;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheToolkit"/> class.
    /// </summary>
    public CacheToolkit(IFileToolkit fileToolkit) => _fileToolkit = fileToolkit;

    /// <inheritdoc/>
    public event EventHandler SessionListChanged;

    /// <inheritdoc/>
    public async Task InitializeCacheSessionsIfNotReadyAsync()
    {
        if (_sessions != null)
        {
            return;
        }

        var data = await _fileToolkit.GetDataFromFileAsync(AppConstants.SavedSessionFileName, new List<SessionMetadata>());
        _sessions = data;
    }

    /// <inheritdoc/>
    public async Task AddOrUpdateSessionMetadataAsync(SessionMetadata metadata)
    {
        await InitializeCacheSessionsIfNotReadyAsync();
        if (_sessions.Contains(metadata))
        {
            var index = _sessions.IndexOf(metadata);
            _sessions.Remove(metadata);
            _sessions.Insert(index, metadata);
        }
        else
        {
            _sessions.Add(metadata);
        }

        var json = JsonSerializer.Serialize(_sessions);
        await _fileToolkit.WriteContentAsync(json, AppConstants.SavedSessionFileName);
        SessionListChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public async Task DeleteSessionAsync(string id)
    {
        var sourceData = _sessions.FirstOrDefault(p => p.Id == id);
        if (sourceData == null)
        {
            return;
        }

        _sessions.Remove(sourceData);
        var json = JsonSerializer.Serialize(_sessions);
        await _fileToolkit.WriteContentAsync(json, AppConstants.SavedSessionFileName);
        await _fileToolkit.DeleteFileAsync(GetSessionFileName(id));
        SessionListChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public async Task<Session> GetSessionByIdAsync(string sessionId)
    {
        var data = await _fileToolkit.GetDataFromFileAsync<Session>(GetSessionFileName(sessionId), default);
        return data;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SessionMetadata>> GetSessionListAsync()
    {
        await InitializeCacheSessionsIfNotReadyAsync();
        return _sessions;
    }

    /// <inheritdoc/>
    public async Task SaveSessionAsync(Session session)
    {
        session.ModifiedTime = DateTimeOffset.Now;
        var json = JsonSerializer.Serialize(session);
        await _fileToolkit.WriteContentAsync(json, GetSessionFileName(session.Id));
    }

    private static string GetSessionFileName(string id)
        => Path.Combine(AppConstants.LocalSessionFolderName, id + ".json");
}
