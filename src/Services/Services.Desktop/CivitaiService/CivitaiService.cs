// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FantasyCopilot.Models.App.Web;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FantasyCopilot.Services;

/// <summary>
/// Civitai service.
/// </summary>
public sealed partial class CivitaiService : ICivitaiService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CivitaiService"/> class.
    /// </summary>
    public CivitaiService(ILogger<CivitaiService> logger)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.None,
        };

        _logger = logger;
        _httpClient = new HttpClient(handler);
        _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = false, NoStore = false };
        _httpClient.DefaultRequestHeaders.Add("User-Agent", AppConstants.DefaultUserAgent);
    }

    /// <inheritdoc/>
    public async Task<CivitaiImageSearchResult> RequestImagesAsync(CivitaiSortType sortType, CivitaiPeriodType periodType, int page, CancellationToken cancellationToken)
    {
        if (page <= 0)
        {
            _logger.LogError($"{nameof(RequestImagesAsync)}: page is less than or equal to 0");
            throw new ArgumentOutOfRangeException(nameof(page));
        }

        var query = $"limit=50&nsfw=None&sort={GetSortTypeText(sortType)}&period={periodType}&page={page}";
        var requestUri = new Uri($"{ImageUrl}?{query}");
        var responseText = await _httpClient.GetStringAsync(requestUri, cancellationToken);
        var pattern = "\"Clip skip\":\"(\\d+)\"";
        var replacement = "\"Clip skip\":$1";
        responseText = System.Text.RegularExpressions.Regex.Replace(responseText, pattern, replacement);
        var result = JsonSerializer.Deserialize<CivitaiImageSearchResult>(responseText);
        return result;
    }

    private static string GetSortTypeText(CivitaiSortType type)
    {
        return type switch
        {
            CivitaiSortType.MostReactions => MostReactions,
            CivitaiSortType.MostComments => MostComments,
            CivitaiSortType.Newest => Newest,
            _ => throw new NotSupportedException(),
        };
    }
}
