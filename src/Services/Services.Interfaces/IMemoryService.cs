// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Models.App.Knowledge;

namespace FantasyCopilot.Services.Interfaces;

/// <summary>
/// The service can handle vector storage, semantic search, based on <see cref="IKernelService"/>.
/// </summary>
public interface IMemoryService
{
    /// <summary>
    /// Connect to the knowledge base.
    /// </summary>
    /// <param name="databasePath">SQLite database path.</param>
    /// <returns>Whether the connection was successful.</returns>
    Task<bool> ConnectSQLiteKnowledgeBaseAsync(string databasePath);

    /// <summary>
    /// Disconnect from the current knowledge base.
    /// </summary>
    void DisconnectSQLiteKnowledgeBase();

    /// <summary>
    /// query memory (context lookup).
    /// </summary>
    /// <param name="query">Query text.</param>
    /// <param name="options">Session options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Message.</returns>
    Task<MessageResponse> QuickSearchMemoryAsync(string query, SessionOptions options, CancellationToken cancellationToken);

    /// <summary>
    /// Search the positioning context from the database.
    /// </summary>
    /// <param name="query">Query text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Knowledge context list.</returns>
    Task<IEnumerable<KnowledgeContext>> AdvancedSearchMemoryAsync(string query, CancellationToken cancellationToken);

    /// <summary>
    /// Pass in the relevant context, after comprehensive summary, provide answers according to the questions.
    /// </summary>
    /// <param name="query">Query text.</param>
    /// <param name="contextList">Knowledge context list.</param>
    /// <param name="options">Session options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Message.</returns>
    Task<MessageResponse> GetAnswerFromContextAsync(string query, IEnumerable<KnowledgeContext> contextList, SessionOptions options, CancellationToken cancellationToken);

    /// <summary>
    /// Import folder to memory.
    /// </summary>
    /// <param name="folderPath">The root directory path.</param>
    /// <param name="searchPattern">Search expression.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task<(int TotalCount, int FailCount)> ImportFolderToMemoryAsync(string folderPath, string searchPattern);

    /// <summary>
    /// Import the contents of the file into memory.
    /// </summary>
    /// <param name="filePath">File path.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task<bool> ImportFileToMemoryAsync(string filePath);

    /// <summary>
    /// Gets the progress of importing files into memory.
    /// </summary>
    /// <returns>imported count and total count.</returns>
    (int ImportedFiles, int TotalFiles) GetImportToMemoryProgress();
}
