// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Data;

namespace RichasyAssistant.App.Resources.Converters;

internal sealed class DoubleToPercentageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is double num
            ? Math.Round(num * 100, 1) + "%"
            : (object)"NaN";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
