// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits;
using FantasyCopilot.Toolkits.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Storage;

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
        RegisterServices();

        var detail = taskInstance.TriggerDetails as AppServiceTriggerDetails;
        _connection = detail.AppServiceConnection;
        _connection.RequestReceived += Connection_RequestReceivedAsync;
    }

    private static void RegisterServices()
    {
        var rootFolder = ApplicationData.Current.LocalFolder;
        var logFolderName = AppConstants.LogFolderName;
        var fullPath = Path.Combine(rootFolder.Path, logFolderName);
        Locator.Current
            .RegisterVariable(typeof(IKernel), new KernelBuilder().Build())
            .RegisterSingleton<ISettingsToolkit, SettingsToolkit>()
            .RegisterSingleton<IKernelService, KernelService>()
            .RegisterLogger(fullPath);
        Locator.Current.Build();
    }

    private async void Connection_RequestReceivedAsync(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
    {
        var msgDeferral = args.GetDeferral();

        var msg = args.Request.Message;
        var returnData = new ValueSet();

        var command = msg["Command"] as string;
        var request = msg["Request"] as string;

        var logger = Locator.Current.GetLogger<AIService>();

        if (!string.IsNullOrEmpty(command))
        {
            _cancellation = new CancellationTokenSource();
            try
            {
                if (command.Equals("quickchat", StringComparison.InvariantCultureIgnoreCase))
                {
                    var data = JsonSerializer.Deserialize<QuickChatPrompt>(request);
                    var kernel = Locator.Current.GetVariable<IKernel>();
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
                logger.LogError(ex, "Error while processing app service");
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
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while handle app service");
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
