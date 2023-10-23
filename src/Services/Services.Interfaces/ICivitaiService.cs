// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.Web;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Services.Interfaces;

/// <summary>
/// Interface definition for Civitai service.
/// </summary>
public interface ICivitaiService
{
    /// <summary>
    /// Request a list of images (50 images at a time).
    /// </summary>
    /// <param name="sortType">Sort by.</param>
    /// <param name="periodType">Period type.</param>
    /// <param name="page">Current page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Image search result.</returns>
    Task<CivitaiImageSearchResult> RequestImagesAsync(CivitaiSortType sortType, CivitaiPeriodType periodType, int page, CancellationToken cancellationToken);
}
