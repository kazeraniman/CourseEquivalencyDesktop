using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using CourseEquivalencyDesktop.ViewModels.Courses;

namespace CourseEquivalencyDesktop.Utility;

/// <summary>
///     Custom converter which returns true if the provided course has an equivalency with the target course.
/// </summary>
public class ShowDeleteEquivalencyConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is not [int id, CoursesPageViewModel coursesPageViewModel])
        {
            return false;
        }

        return coursesPageViewModel.EquivalentCourse is not null &&
               coursesPageViewModel.EquivalentCourse.Equivalencies.Any(equivalentCourse => equivalentCourse.Id == id);
    }
}
