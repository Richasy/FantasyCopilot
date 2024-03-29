﻿// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Data;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.App.Resources.Converters;

internal sealed class OnlinePromptSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var source = (OnlinePromptSource)value;
        var resourceToolkit = Locator.Current.GetService<IResourceToolkit>();
        var text = source switch
        {
            OnlinePromptSource.AwesomePrompt => resourceToolkit.GetLocalizedString(StringNames.AwesomePrompt),
            OnlinePromptSource.AwesomePromptZh => resourceToolkit.GetLocalizedString(StringNames.AwesomePromptZh),
            OnlinePromptSource.FlowGptEnTrending => resourceToolkit.GetLocalizedString(StringNames.FlowGptEnTrending),
            OnlinePromptSource.FlowGptZhTrending => resourceToolkit.GetLocalizedString(StringNames.FlowGptZhTrending),
            _ => throw new NotImplementedException(),
        };

        return text;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
