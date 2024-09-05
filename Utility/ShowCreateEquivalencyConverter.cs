using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using CourseEquivalencyDesktop.ViewModels.Courses;

namespace CourseEquivalencyDesktop.Utility;

/// <summary>
///     Custom converter which returns true if the provided course does not have an equivalency with the target course.
/// </summary>
public class ShowCreateEquivalencyConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is not [int id, CoursesPageViewModel coursesPageViewModel])
        {
            return false;
        }

        return coursesPageViewModel.EquivalentCourse is null ||
               coursesPageViewModel.EquivalentCourse.Equivalencies.All(equivalentCourse =>
                   equivalentCourse.Id != id);
    }
}
