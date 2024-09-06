using System;
using System.Collections;
using CourseEquivalencyDesktop.Models;

namespace CourseEquivalencyDesktop.Utility;

/// <summary>
///     Custom comparer which compares due dates for study plans to the current date and sorts them as such:
///     1. Nearest to farthest dates in the future.
///     2. Null dates
///     3. Farthest to nearest dates in the past.
/// </summary>
public class StudyPlanDueDateComparer : IComparer
{
    public int Compare(object? x, object? y)
    {
        var dateTimeX = (x as StudyPlan)?.DueDate;
        var dateTimeY = (y as StudyPlan)?.DueDate;

        if (!dateTimeX.HasValue && !dateTimeY.HasValue)
        {
            return 0;
        }

        var today = DateTime.Today;
        if (!dateTimeX.HasValue)
        {
            return dateTimeY!.Value >= today ? 1 : -1;
        }

        if (!dateTimeY.HasValue)
        {
            return dateTimeX.Value >= today ? -1 : 1;
        }

        var xTimeDiff = dateTimeX.Value - today;
        var yTimeDiff = dateTimeY.Value - today;
        var xIsFuture = dateTimeX.Value >= today;
        var yIsFuture = dateTimeY.Value >= today;

        return xIsFuture switch
        {
            true when !yIsFuture => -1,
            false when yIsFuture => 1,
            true when yIsFuture => xTimeDiff.CompareTo(yTimeDiff),
            false when !yIsFuture => xTimeDiff.CompareTo(yTimeDiff),
            _ => 0
        };
    }
}
