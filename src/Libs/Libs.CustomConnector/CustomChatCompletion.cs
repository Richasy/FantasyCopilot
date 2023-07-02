// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FantasyCopilot.Models.App.Connectors;
using FantasyCopilot.Models.Constants;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.ChatCompletion;

namespace FantasyCopilot.Libs.CustomConnector;

/// <summary>
/// Custom chat completion service.
/// </summary>
public sealed class CustomChatCompletion : IChatCompletion
{
    private readonly ConnectorConfig _connectorConfig;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomChatCompletion"/> class.
    /// </summary>
    /// <param name="config">Connector config.</param>
    public CustomChatCompletion(ConnectorConfig config)
    {
        _connectorConfig = config;
        _httpClient = new HttpClient();
    }

    /// <inheritdoc/>
    public ChatHistory CreateNewChat(string? instructions = null)
    {
        var chatHistory = new ChatHistory();
        if (!string.IsNullOrEmpty(instructions))
        {
            chatHistory.AddSystemMessage(instructions);
        }

        return chatHistory;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<IChatResult>> GetChatCompletionsAsync(ChatHistory chat, ChatRequestSettings? requestSettings = null, CancellationToken cancellationToken = default)
    {
        VerifyNotNull(requestSettings);
        var config = _connectorConfig.Features
            .First(p => p.Type == ConnectorConstants.ChatType)
            .Endpoints
            .First(p => p.Type == ConnectorConstants.ChatRestType);
        var url = new Uri(_connectorConfig.BaseUrl + config.Path);
        var requestData = GetRequest(chat, requestSettings);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(requestData),
        };
        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        var resultContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<MessageResult>(resultContent);
        return result.IsError
            ? throw new AIException(AIException.ErrorCodes.ServiceError, result.Content ?? "Something error")
            : new List<IChatResult> { new CustomChatCompletionResult(result) }.AsReadOnly();
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<IChatStreamingResult> GetStreamingChatCompletionsAsync(ChatHistory chat, ChatRequestSettings? requestSettings = null, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    private static void VerifyNotNull(object obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException("Parameter is null");
        }
    }

    private static Request GetRequest(ChatHistory chat, ChatRequestSettings requestSettings)
    {
        var userMsg = chat.LastOrDefault(p => p.Role == AuthorRole.User)
            ?? throw new AIException(AIException.ErrorCodes.InvalidRequest);
        var history = new List<Message>();
        for (var i = 0; i < chat.Messages.Count - 1; i++)
        {
            var item = chat.Messages[i];
            var msg = new Message { Content = item.Content, Role = item.Role.ToString() };
            history.Add(msg);
        }

        var settings = new RequestSettings
        {
            FrequencyPenalty = requestSettings.FrequencyPenalty,
            MaxResponseTokens = requestSettings.MaxTokens,
            PresencePenalty = requestSettings.PresencePenalty,
            Temperature = requestSettings.Temperature,
            TopP = requestSettings.TopP,
        };

        return new Request
        {
            Message = userMsg.Content,
            History = history,
            Settings = settings,
        };
    }

    internal class CustomChatCompletionResult : IChatResult
    {
        private readonly MessageResult _messageResult;

        public CustomChatCompletionResult(MessageResult result)
            => _messageResult = result;

        public Task<ChatMessageBase> GetChatMessageAsync(CancellationToken cancellationToken = default)
            => Task.FromResult((ChatMessageBase)new ChatMessage(AuthorRole.Assistant, _messageResult.Content));
    }

    internal class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    internal class RequestSettings
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("maxResponseTokens")]
        public int MaxResponseTokens { get; set; }

        [JsonPropertyName("topP")]
        public double TopP { get; set; }

        [JsonPropertyName("frequencyPenalty")]
        public double FrequencyPenalty { get; set; }

        [JsonPropertyName("presencePenalty")]
        public double PresencePenalty { get; set; }
    }

    internal class Request
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("history")]
        public List<Message> History { get; set; }

        [JsonPropertyName("settings")]
        public RequestSettings Settings { get; set; }
    }

    internal class MessageResult
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("isError")]
        public bool IsError { get; set; }
    }

    private sealed class ChatMessage : ChatMessageBase
    {
        public ChatMessage(AuthorRole authorRole, string content)
            : base(authorRole, content)
        {
        }
    }
}
