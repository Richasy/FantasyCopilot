// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace FantasyCopilot.Services;

/// <summary>
/// Civitai service.
/// </summary>
public sealed partial class CivitaiService
{
    private const string ImageUrl = "https://civitai.com/api/v1/images";
    private const string MostReactions = "Most%20Reactions";
    private const string MostComments = "Most%20Comments";
    private const string Newest = "Newest";

    private readonly HttpClient _httpClient;
    private readonly ILogger<CivitaiService> _logger;
}
