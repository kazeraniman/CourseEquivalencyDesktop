using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Styling;

namespace CourseEquivalencyDesktop.Utility;

public class IconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string iconName)
        {
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        Application.Current!.TryGetResource(iconName, ThemeVariant.Default, out var iconResource);
        return iconResource as StreamGeometry;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
