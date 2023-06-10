// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using FantasyCopilot.Models.App;
using Microsoft.SemanticKernel.AI.ChatCompletion;

namespace FantasyCopilot.Libs.NativeSkills;

internal static class StaticHelpers
{
    internal static T GetStepParameters<T>(this WorkflowContext context)
    {
        var data = context.StepParameters[context.CurrentStepIndex];
        return string.IsNullOrEmpty(data)
            ? default
            : JsonSerializer.Deserialize<T>(data);
    }

    internal static bool TryGetTokenLimit(this string errorMessage, out int maxTokens, out int messageTokens)
    {
        var pattern = @"maximum context length.*?(\d+).*?(\d+)";
        var match = Regex.Match(errorMessage, pattern);
        maxTokens = 0;
        messageTokens = 0;
        if (match.Success)
        {
            maxTokens = int.Parse(match.Groups[1].Value);
            messageTokens = int.Parse(match.Groups[2].Value);
            return true;
        }

        return false;
    }

    internal static bool TryRemoveEarlierMessage(List<ChatHistory.Message> messages, int maxTokens, int messageTokens)
    {
        // If there are too few rounds, then message generation cannot continue by deleting messages.
        if (messages.Count <= 1 || !messages.Any(p => p.AuthorRole == ChatHistory.AuthorRoles.Assistant))
        {
            return false;
        }

        var startIndex = messages.FirstOrDefault()?.AuthorRole == ChatHistory.AuthorRoles.System ? 1 : 0;
        var difference = messageTokens - maxTokens;
        var endIndex = -1;
        var tempTokenRecord = 0;
        for (var i = startIndex; i < messages.Count; i++)
        {
            var message = messages[i];

            // Currently, there is no token algorithm that is suitable for most languages,
            // so this is only an estimate and there is some error.
            tempTokenRecord += message.Content.Length * 2;
            if (tempTokenRecord >= difference)
            {
                endIndex = i;
                break;
            }
        }

        if (endIndex == -1)
        {
            return false;
        }

        messages.RemoveRange(startIndex, endIndex - startIndex + 1);
        return true;
    }
}
