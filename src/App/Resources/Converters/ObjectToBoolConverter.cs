// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Data;

namespace RichasyAssistant.App.Resources.Converters
{
    /// <summary>
    /// Object to Boolean converter.
    /// </summary>
    /// <remarks>
    /// Returns <c>True</c> when the object is not empty.
    /// </remarks>
    public class ObjectToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Whether to invert the result.
        /// </summary>
        /// <remarks>
        /// After inversion, return <c>True</c> when the string is empty, otherwise return <c>False</c>.
        /// </remarks>
        public bool IsReverse { get; set; }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var result = false;
            if (value != null)
            {
                if (value is string str)
                {
                    result = !string.IsNullOrEmpty(str);
                }
                else if (value is bool b)
                {
                    result = b;
                }
                else
                {
                    result = true;
                }
            }

            if (IsReverse)
            {
                result = !result;
            }

            return result;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
