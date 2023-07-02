// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Libs.CustomConnector;
using FantasyCopilot.Models.App.Connectors;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;

namespace FantasyCopilot.Services;

internal static class CustomAIKernelBuilderExtensions
{
    public static KernelBuilder WithCustomChatCompletionService(this KernelBuilder builder, ConnectorConfig connectorConfig)
    {
        var service = new CustomChatCompletion(connectorConfig);
        builder.WithAIService<IChatCompletion>(default, service);
        return builder;
    }
}
