using System.Collections.Generic;
using System.Linq;
using CourseEquivalencyDesktop.Models;

namespace CourseEquivalencyDesktop.Utility;

/// <summary>
///     Custom comparer which sorts courses based on their equivalency so that they show up in the correct order on
///     exports.
/// </summary>
/// <param name="courseList">The source list used for ordering equivalencies (contains indices to match in the pair).</param>
public class EquivalentCourseComparer(IList<Course> courseList) : IComparer<Course>
{
    public int Compare(Course? x, Course? y)
    {
        var isXNull = x is null;
        var isYNull = y is null;

        if (isXNull && isYNull)
        {
            return 0;
        }

        if (isXNull)
        {
            return 1;
        }

        if (isYNull)
        {
            return -1;
        }

        var courseXIndex =
            courseList.FindIndex(course => x!.Equivalencies.Any(equivalent => equivalent.Id == course.Id));
        var courseYIndex =
            courseList.FindIndex(course => y!.Equivalencies.Any(equivalent => equivalent.Id == course.Id));

        if (courseXIndex != -1 && courseYIndex == -1)
        {
            return -1;
        }

        if (courseXIndex == -1 && courseYIndex != -1)
        {
            return 1;
        }

        if (courseXIndex < courseYIndex)
        {
            return -1;
        }

        if (courseXIndex > courseYIndex)
        {
            return 1;
        }

        if (x!.Id < y!.Id)
        {
            return -1;
        }

        return 1;
    }
}
