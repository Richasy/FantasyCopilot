// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace AppServices;

/// <summary>
/// Quick chat service.
/// </summary>
public sealed class QuickChat : IBackgroundTask
{
    private readonly string[] _names = new string[] { "Alice", "Bob" };
    private readonly int[] _ages = new int[] { 21, 22 };
    private BackgroundTaskDeferral _deferral;
    private AppServiceConnection _connection;

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
        var greatIndex = msg["ID"] as int?;

        if (greatIndex.HasValue && greatIndex.Value >= 0 && greatIndex.Value < _names.GetLength(0))
        {
            switch (command)
            {
                case "Name":
                    returnData.Add("Result", _names[greatIndex.Value]);
                    returnData.Add("Status", "ok");
                    break;

                case "Age":
                    returnData.Add("Result", _ages[greatIndex.Value]);
                    returnData.Add("Status", "ok");
                    break;

                default:
                    returnData.Add("Status", "fail");
                    break;
            }
        }
        else
        {
            returnData.Add("fail", "Index out of range");
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
        => _deferral?.Complete();
}
