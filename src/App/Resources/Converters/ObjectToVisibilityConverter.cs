// Copyright (c) Fantasy Copilot. All rights reserved.

using Microsoft.UI.Xaml.Data;

namespace FantasyCopilot.App.Resources.Converters
{
    /// <summary>
    /// Object to visibility converter. Returns Collapsed when the object is empty.
    /// </summary>
    public class ObjectToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Whether to invert the result.
        /// </summary>
        public bool IsReverse { get; set; }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isShow = true;
            if (value == null)
            {
                isShow = false;
            }
            else if (value is string str)
            {
                isShow = !string.IsNullOrEmpty(str);
            }

            if (IsReverse)
            {
                isShow = !isShow;
            }

            return isShow ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
