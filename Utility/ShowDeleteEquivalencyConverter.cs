using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using CourseEquivalencyDesktop.ViewModels.Courses;

namespace CourseEquivalencyDesktop.Utility;

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
