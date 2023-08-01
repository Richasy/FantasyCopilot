// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using FantasyCopilot.Models.App.Image;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using Microsoft.Extensions.Logging;

namespace FantasyCopilot.Services;

/// <summary>
/// Stable diffusion image service.
/// </summary>
public sealed partial class StableDiffusionImageService : IImageService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StableDiffusionImageService"/> class.
    /// </summary>
    public StableDiffusionImageService(
        ISettingsToolkit settingsToolkit,
        ILogger<StableDiffusionImageService> logger)
    {
        _settingsToolkit = settingsToolkit;
        _logger = logger;
        _httpClient = new HttpClient();
    }

    /// <inheritdoc/>
    public bool HasValidConfig => _hasValidConfig;

    /// <inheritdoc/>
    public async Task<(byte[] ImageData, string Context)> GenerateImageAsync(string prompt, string negativePrompt, GenerateOptions options, CancellationToken cancellationToken)
    {
        var obj = new
        {
            prompt,
            negative_prompt = negativePrompt,
            seed = options.Seed,
            sampler_name = options.Sampler,
            batch_size = 1,
            n_iter = 1,
            steps = options.Steps,
            cfg_scale = options.CfgScale,
            width = options.Width,
            height = options.Height,
            send_images = true,
            save_images = false,
            override_settings = new
            {
                sd_model_checkpoint = options.Model,
            },
        };

        var requestJson = JsonSerializer.Serialize(obj);
        var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
        try
        {
            var response = await _httpClient.PostAsync(Text2ImageUrl, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonSerializer.Deserialize<JsonElement>(json);
            if (doc.TryGetProperty("images", out var detail))
            {
                var imageArr = detail.Deserialize<string[]>();
                var base64Str = imageArr.First();
                var bytes = Convert.FromBase64String(base64Str);

                var context = string.Empty;
                if (doc.TryGetProperty("info", out var infoEle))
                {
                    var infoJson = infoEle.GetString();
                    var infoDetailEle = JsonSerializer.Deserialize<JsonElement>(infoJson);
                    if (infoDetailEle.TryGetProperty("infotexts", out var infoExt))
                    {
                        var extArr = infoExt.Deserialize<string[]>();
                        context = extArr.First();
                    }
                }

                return (bytes, context);
            }

            return default;
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation($"User canceled image generation");
            await _httpClient.PostAsync(InterruptUrl, default, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while generating the image");
            throw;
        }

        return default;
    }

    /// <inheritdoc/>
    public async Task<ExtraModelPackage> GetExtraModelsAsync()
    {
        await InitializeExtraModelsIfNotReadyAsync();
        return _extraModels;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Model>> GetModelsAsync()
    {
        await InitializeModelsIfNotReadyAsync();
        return _models;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Sampler>> GetSamplersAsync()
    {
        await InitializeSamplersIfNotReadyAsync();
        return _samplers;
    }

    /// <inheritdoc/>
    public async Task InitializeExtraModelsIfNotReadyAsync(bool force = false)
    {
        if (force)
        {
            var response = await _httpClient.PostAsync(RefreshLorasUrl, default);
            response.EnsureSuccessStatusCode();
            _extraModels = default;
        }

        if (_extraModels != null)
        {
            return;
        }

        var model = new ExtraModelPackage();
        var embeddingTask = Task.Run(async () =>
        {
            var embeddings = new List<string>();
            var json = await _httpClient.GetStringAsync(GetEmbeddingsUrl);
            var root = JsonSerializer.Deserialize<JsonElement>(json);
            var enumerator = root.GetProperty("loaded").EnumerateObject();
            while (enumerator.MoveNext())
            {
                embeddings.Add(enumerator.Current.Name);
            }

            model.Embeddings = embeddings;
        });

        var loraTask = Task.Run(async () =>
        {
            var loras = new List<string>();
            var json = await _httpClient.GetStringAsync(GetLorasUrl);
            var jarr = JsonArray.Parse(json);
            foreach (var item in jarr.AsArray())
            {
                var jobj = item.AsObject();
                if (jobj.ContainsKey("name"))
                {
                    loras.Add(jobj["name"].GetValue<string>());
                }
            }

            model.Loras = loras;
        });

        await Task.WhenAll(embeddingTask, loraTask);
        _extraModels = model;
    }

    /// <inheritdoc/>
    public async Task InitializeModelsIfNotReadyAsync(bool force = false)
    {
        if (force)
        {
            var response = await _httpClient.PostAsync(RefreshModelsUrl, default);
            response.EnsureSuccessStatusCode();
            _models = default;
        }

        if (_models != null)
        {
            return;
        }

        var json = await _httpClient.GetStringAsync(GetModelsUrl);
        var models = JsonSerializer.Deserialize<List<Model>>(json);
        _models = models;
    }

    /// <inheritdoc/>
    public async Task InitializeSamplersIfNotReadyAsync()
    {
        var json = await _httpClient.GetStringAsync(GetSamplersUrl);
        var samplers = JsonSerializer.Deserialize<List<Sampler>>(json);
        _samplers = samplers;
    }

    /// <inheritdoc/>
    public async Task<bool> PingAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(PingUrl);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public void ReloadConfig()
    {
        _stableDiffusionUrl = default;
        CheckConfig();
    }

    private void CheckConfig()
    {
        var hasKey = _settingsToolkit.IsSettingKeyExist(SettingNames.StableDiffusionUrl);
        _hasValidConfig = hasKey;

        if (_hasValidConfig && string.IsNullOrEmpty(_stableDiffusionUrl))
        {
            _stableDiffusionUrl = _settingsToolkit.ReadLocalSetting(SettingNames.StableDiffusionUrl, string.Empty).TrimEnd('/');
        }
    }
}
