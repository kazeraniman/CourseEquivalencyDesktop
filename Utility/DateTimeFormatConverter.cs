using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace CourseEquivalencyDesktop.Utility;

/// <summary>
///     Custom converter which formats the datetime to the provided format string.
/// </summary>
public class DateTimeFormatConverter : IValueConverter
{
    public string DateTimeFormat { get; set; } = "yyyy-M-d h:mm:ss tt";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is DateTime dateTime ? dateTime.ToString(DateTimeFormat) : string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return DateTime.TryParse(value?.ToString() ?? string.Empty, out var result) ? result : null;
    }
}
