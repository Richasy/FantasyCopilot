// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using System.Net.Http;
using RichasyAssistant.Models.App.Image;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.Libs.Services;

/// <summary>
/// Stable diffusion image service.
/// </summary>
public sealed partial class StableDiffusionImageService
{
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly HttpClient _httpClient;
    private bool _hasValidConfig;
    private string _stableDiffusionUrl;

    private List<Sampler> _samplers;
    private List<Model> _models;
    private ExtraModelPackage _extraModels;

    /// <summary>
    /// Instance.
    /// </summary>
    public static StableDiffusionImageService Instance { get; } = new StableDiffusionImageService();

    private string PingUrl => $"{_stableDiffusionUrl}/internal/ping";

    private string GetModelsUrl => $"{_stableDiffusionUrl}/sdapi/v1/sd-models";

    private string RefreshModelsUrl => $"{_stableDiffusionUrl}/sdapi/v1/reload-checkpoint";

    private string GetSamplersUrl => $"{_stableDiffusionUrl}/sdapi/v1/samplers";

    private string GetEmbeddingsUrl => $"{_stableDiffusionUrl}/sdapi/v1/embeddings";

    private string GetLorasUrl => $"{_stableDiffusionUrl}/sdapi/v1/loras";

    private string RefreshLorasUrl => $"{_stableDiffusionUrl}/sdapi/v1/refresh-loras";

    private string InterruptUrl => $"{_stableDiffusionUrl}/sdapi/v1/interrupt";

    private string Text2ImageUrl => $"{_stableDiffusionUrl}/sdapi/v1/txt2img";
}
