using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using CourseEquivalencyDesktop.ViewModels.Courses;

namespace CourseEquivalencyDesktop.Utility;

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
