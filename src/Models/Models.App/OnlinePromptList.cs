// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.App;

/// <summary>
/// Online prompt source data.
/// </summary>
public sealed class OnlinePromptList
{
    /// <summary>
    /// When cache data.
    /// </summary>
    public DateTimeOffset CacheTime { get; set; }

    /// <summary>
    /// Web source.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Data list.
    /// </summary>
    public List<OnlinePrompt> List { get; set; }
}
