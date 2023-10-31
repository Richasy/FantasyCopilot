// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using RichasyAssistant.Models.App.Gpt;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Session caching service.
/// </summary>
public sealed partial class SessionService
{
    private readonly List<Message> _messages;
    private SessionOptions _sessionOptions;

    /// <summary>
    /// Instance.
    /// </summary>
    public static SessionService Instance { get; } = new SessionService();
}
