using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace CourseEquivalencyDesktop.Utility;

/// <summary>
///     Custom converter which reverses a string.
/// </summary>
public class StringReverseConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var stringValue = value?.ToString();
        return Utility.ReverseString(stringValue);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
