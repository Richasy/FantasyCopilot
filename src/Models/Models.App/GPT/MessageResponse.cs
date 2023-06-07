// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.App.Gpt;

/// <summary>
/// The message response returned by the service.
/// </summary>
public sealed class MessageResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageResponse"/> class.
    /// </summary>
    /// <param name="isError">Is something wrong.</param>
    /// <param name="content">message.</param>
    /// <param name="additionalContent">Extra info.</param>
    public MessageResponse(bool isError, string content, string additionalContent = default)
    {
        IsError = isError;
        Content = content;
        AdditionalContent = additionalContent;
    }

    /// <summary>
    /// Whether the return is an error message.
    /// </summary>
    public bool IsError { get; }

    /// <summary>
    /// Message.
    /// </summary>
    public string Content { get; }

    /// <summary>
    /// Additional content.
    /// </summary>
    public string AdditionalContent { get; }
}
