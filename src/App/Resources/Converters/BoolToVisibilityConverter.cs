// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Data;

namespace RichasyAssistant.App.Resources.Converters
{
    /// <summary>
    /// <see cref="bool"/> to <see cref="Visibility"/>.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Whether to invert the value.
        /// </summary>
        public bool IsReverse { get; set; }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var vis = Visibility.Visible;
            if (value is bool v)
            {
                if (IsReverse)
                {
                    vis = v ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    vis = v ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            return vis;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
