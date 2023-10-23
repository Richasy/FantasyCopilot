// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.AI.TextCompletion;
using RichasyAssistant.Libs.CustomConnector;
using RichasyAssistant.Models.App.Connectors;

namespace RichasyAssistant.Services;

internal static class CustomAIKernelBuilderExtensions
{
    public static KernelBuilder WithCustomChatCompletionService(this KernelBuilder builder, ConnectorConfig connectorConfig)
    {
        var service = new CustomChatCompletion(connectorConfig);
        builder.WithAIService<IChatCompletion>(default, service);
        return builder;
    }

    public static KernelBuilder WithCustomTextCompletionService(this KernelBuilder builder, ConnectorConfig connectorConfig)
    {
        var service = new CustomTextCompletion(connectorConfig);
        builder.WithAIService<ITextCompletion>(default, service);
        return builder;
    }

    public static KernelBuilder WithCustomTextEmbeddingGenerationService(this KernelBuilder builder, ConnectorConfig connectorConfig)
    {
        var service = new CustomTextEmbeddingGeneration(connectorConfig);
        builder.WithAIService<ITextEmbeddingGeneration>(default, service);
        return builder;
    }
}
