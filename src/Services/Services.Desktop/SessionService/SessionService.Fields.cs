// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.Generic;
using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FantasyCopilot.Services;

/// <summary>
/// Session caching service.
/// </summary>
public sealed partial class SessionService
{
    private readonly IChatService _chatService;
    private readonly List<Message> _messages;
    private readonly ILogger<SessionService> _logger;
    private SessionOptions _sessionOptions;
}
