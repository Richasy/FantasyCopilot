﻿// <auto-generated />
// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Text.Json.Serialization;

namespace FantasyCopilot.Models.App;

public sealed class AppConfig
{
    [JsonPropertyName("azureOpenAI")]
    public AzureOpenAIConfig AzureOpenAI { get; set; }

    [JsonPropertyName("openAI")]
    public OpenAIConfig OpenAI { get; set; }

    [JsonPropertyName("huggingFace")]
    public HuggingFaceConfig HuggingFace { get; set; }

    [JsonPropertyName("azureVoice")]
    public RegionConfig AzureVoice { get; set; }

    [JsonPropertyName("azureTranslate")]
    public RegionConfig AzureTranslate { get; set; }

    [JsonPropertyName("baiduTranslate")]
    public BaiduTranslateConfig BaiduTranslate { get; set; }

    [JsonPropertyName("stableDiffusion")]
    public UrlConfig StableDiffusion { get; set; }

    public sealed class AzureOpenAIConfig : ConfigBase
    {
        [JsonPropertyName("endpoint")]
        public string Endpoint { get; set; }

        [JsonPropertyName("chatModelName")]
        public string ChatModelName { get; set; }

        [JsonPropertyName("embeddingModelName")]
        public string EmbeddingModelName { get; set; }

        [JsonPropertyName("completionModelName")]
        public string CompletionModelName { get; set; }
    }

    public sealed class OpenAIConfig : ConfigBase
    {
        [JsonPropertyName("organization")]
        public string Organization { get; set; }

        [JsonPropertyName("chatModelName")]
        public string ChatModelName { get; set; }

        [JsonPropertyName("embeddingModelName")]
        public string EmbeddingModelName { get; set; }

        [JsonPropertyName("completionModelName")]
        public string CompletionModelName { get; set; }
    }

    public sealed class HuggingFaceConfig : ConfigBase
    {
        [JsonPropertyName("embeddingModelName")]
        public string EmbeddingModelName { get; set; }

        [JsonPropertyName("completionModelName")]
        public string CompletionModelName { get; set; }

        [JsonPropertyName("embeddingEndpoint")]
        public string EmbeddingEndpoint { get; set; }

        [JsonPropertyName("completionEndpoint")]
        public string CompletionEndpoint { get; set; }
    }

    public sealed class RegionConfig : ConfigBase
    {
        [JsonPropertyName("region")]
        public string Region { get; set; }
    }

    public sealed class UrlConfig
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public sealed class BaiduTranslateConfig : ConfigBase
    {
        [JsonPropertyName("appId")]
        public string AppId { get; set; }
    }

    public class ConfigBase
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
    }
}
