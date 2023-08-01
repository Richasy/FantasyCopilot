// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Text.Json;
using System.Threading;
using FantasyCopilot.Models.App;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace FantasyCopilot.AppServices;

/// <summary>
/// Quick chat service.
/// </summary>
public sealed class AIService : IBackgroundTask
{
    private BackgroundTaskDeferral _deferral;
    private AppServiceConnection _connection;
    private CancellationTokenSource _cancellation;

    /// <inheritdoc/>
    public void Run(IBackgroundTaskInstance taskInstance)
    {
        _deferral = taskInstance.GetDeferral();

        taskInstance.Canceled += TaskInstance_Canceled;

        var detail = taskInstance.TriggerDetails as AppServiceTriggerDetails;
        _connection = detail.AppServiceConnection;
        _connection.RequestReceived += Connection_RequestReceivedAsync;
    }

    private async void Connection_RequestReceivedAsync(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
    {
        var msgDeferral = args.GetDeferral();

        var msg = args.Request.Message;
        var returnData = new ValueSet();

        var command = msg["Command"] as string;
        var request = msg["Request"] as string;

        if (!string.IsNullOrEmpty(command))
        {
            _cancellation = new CancellationTokenSource();
            try
            {
                if (command.Equals("quickchat", StringComparison.InvariantCultureIgnoreCase))
                {
                    var data = JsonSerializer.Deserialize<QuickChatPrompt>(request);
                    var kernel = await Utils.GetSemanticKernelAsync();

                    if (kernel == null)
                    {
                        returnData.Add("Error", "Kernel is not available");
                    }

                    if (data.UseChat)
                    {
                        var chatCompletion = kernel.GetService<IChatCompletion>() ?? throw new Exception("Chat completion service is not available");
                        var requestSettings = new ChatRequestSettings
                        {
                            MaxTokens = data.MaxResponseTokens == 0 ? 600 : data.MaxResponseTokens,
                            Temperature = data.Temperature,
                            FrequencyPenalty = 1,
                            PresencePenalty = 1,
                            TopP = 0,
                        };
                        var chatHistory = chatCompletion.CreateNewChat();
                        chatHistory.AddUserMessage(data.ActualMessage);
                        var response = await chatCompletion.GenerateMessageAsync(chatHistory, requestSettings, _cancellation.Token);
                        returnData.Add("Response", response);
                    }
                    else
                    {
                        var textCompletion = kernel.GetService<ITextCompletion>() ?? throw new Exception("Text completion service is not available");
                        var requestSettings = new CompleteRequestSettings
                        {
                            MaxTokens = data.MaxResponseTokens == 0 ? 600 : data.MaxResponseTokens,
                            Temperature = data.Temperature,
                            FrequencyPenalty = 1,
                            PresencePenalty = 1,
                            TopP = 0,
                        };

                        var response = await textCompletion.CompleteAsync(data.ActualMessage, requestSettings, _cancellation.Token);
                        returnData.Add("Response", response);
                    }
                }
            }
            catch (Exception ex)
            {
                returnData.Add("Error", ex.Message);
            }

            _cancellation = null;
        }
        else
        {
            returnData.Add("Error", "Invalid command");
        }

        try
        {
            await args.Request.SendResponseAsync(returnData);
        }
        catch (Exception)
        {
        }
        finally
        {
            msgDeferral.Complete();
        }
    }

    private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
    {
        _cancellation?.Cancel();
        _deferral?.Complete();
    }
}
