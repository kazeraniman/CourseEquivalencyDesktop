using System;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace CourseEquivalencyDesktop.Utility;

/// <summary>
/// A comparer to allow for enums to be bound to radio buttons.
/// </summary>
/// <remarks>
/// This was taken from the StackOverflow post found here: https://stackoverflow.com/a/2908885/2512688
/// The following modifications were made:
/// 1. Made the value and the parameter nullable.
/// 2. Changed from Binding.DoNothing to BindingOperations.DoNothing as the former could not be found.
/// 3. Coalesced the return value of ConvertBack to avoid null errors.
/// </remarks>
public class ComparisonConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        return value?.Equals(parameter);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        return (value?.Equals(true) == true ? parameter : BindingOperations.DoNothing) ?? false;
    }
}
