// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Threading;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.AI.TextCompletion;

namespace FantasyCopilot.App.Controls;

/// <summary>
/// 快速对话框.
/// </summary>
public sealed partial class QuickChatDialog : ContentDialog
{
    private readonly QuickChatPrompt _prompt;
    private CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuickChatDialog"/> class.
    /// </summary>
    public QuickChatDialog(QuickChatPrompt prompt)
    {
        _prompt = prompt;
        InitializeComponent();
        DisplayMessageBlock.Text = prompt.DisplayMessage;
        Loaded += OnLoadedAsync;
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        LoadingRing.IsActive = true;
        var logger = Locator.Current.GetLogger<QuickChatDialog>();
        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var kernel = Locator.Current.GetVariable<IKernel>();
            if (_prompt.UseChat)
            {
                var chatCompletion = kernel.GetService<IChatCompletion>() ?? throw new Exception("Chat completion service is not available");
                var requestSettings = new ChatRequestSettings
                {
                    MaxTokens = _prompt.MaxResponseTokens == 0 ? 600 : _prompt.MaxResponseTokens,
                    Temperature = _prompt.Temperature,
                    FrequencyPenalty = 1,
                    PresencePenalty = 1,
                    TopP = 0,
                };
                var chatHistory = chatCompletion.CreateNewChat();
                chatHistory.AddUserMessage(_prompt.ActualMessage);
                var response = await chatCompletion.GenerateMessageAsync(chatHistory, requestSettings, _cancellationTokenSource.Token);
                ResponseBlock.Text = response;
            }
            else
            {
                var textCompletion = kernel.GetService<ITextCompletion>() ?? throw new Exception("Text completion service is not available");
                var requestSettings = new CompleteRequestSettings
                {
                    MaxTokens = _prompt.MaxResponseTokens == 0 ? 600 : _prompt.MaxResponseTokens,
                    Temperature = _prompt.Temperature,
                    FrequencyPenalty = 1,
                    PresencePenalty = 1,
                    TopP = 0,
                };

                var response = await textCompletion.CompleteAsync(_prompt.ActualMessage, requestSettings, _cancellationTokenSource.Token);
                ResponseBlock.Text = response;
            }
        }
        catch (Exception ex)
        {
            ResponseBlock.Text = ex.Message;
            logger.LogError(ex, "Quick chat failed");
        }

        _cancellationTokenSource = default;
        LoadingRing.IsActive = false;
    }

    private void OnCloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        => _cancellationTokenSource?.Cancel();
}
