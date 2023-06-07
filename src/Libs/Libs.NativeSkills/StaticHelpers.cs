// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Text.Json;
using FantasyCopilot.Models.App;

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
}
