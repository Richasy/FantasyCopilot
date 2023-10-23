// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RichasyAssistant.Models.App.Gpt;
using RichasyAssistant.Services.Interfaces;

namespace RichasyAssistant.Services;

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
