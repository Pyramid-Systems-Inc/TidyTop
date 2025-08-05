using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace TidyTop.App.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is System.Drawing.Color color)
            {
                return new SolidColorBrush(Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}