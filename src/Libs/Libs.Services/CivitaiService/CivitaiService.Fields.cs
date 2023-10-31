// Copyright (c) Richasy Assistant. All rights reserved.

using System.Net.Http;

namespace RichasyAssistant.Libs.Services;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="CivitaiService"/> class.
    /// </summary>
    public static CivitaiService Instance { get; } = new CivitaiService();
}
