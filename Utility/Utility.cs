using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using CourseEquivalencyDesktop.Models;

namespace CourseEquivalencyDesktop.Utility;

/// <summary>
///     Holds an assortment of utility methods.
///     If there are sufficiently many related methods, consider extracting into their own class.
/// </summary>
public static class Utility
{
    #region Constants
    private const double DEFAULT_APPROXIMATELY_EQUAL_TOLERANCE = 1E-15;
    #endregion

    #region Design
    /// <summary>
    ///     Ensures that we are currently using Design Mode.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if we are running ouside of Design Mode.</exception>
    /// <remarks>
    ///     This would be nicer to have as an attribute, but attribute interception appears to not work on constructors from my
    ///     reading.
    /// </remarks>
    public static void AssertDesignMode()
    {
        if (Design.IsDesignMode)
        {
            return;
        }

        throw new InvalidOperationException("This method must only be used in Design Mode!");
    }
    #endregion

    #region Collections
    /// <summary>
    ///     Adds all the provided items to the provided collection.
    /// </summary>
    /// <param name="observableCollection">The collection to which the items should be added.</param>
    /// <param name="newItems">The items to add.</param>
    public static void AddRange<T>(this ObservableCollection<T> observableCollection, IEnumerable<T> newItems)
    {
        foreach (var item in newItems)
        {
            observableCollection.Add(item);
        }
    }

    /// <summary>
    ///     Finds the index of the first item to match the predicate and -1 if none exist.
    /// </summary>
    /// <param name="list">The list to search.</param>
    /// <param name="predicate">The predicate to match.</param>
    /// <returns>Index of the first item to match the predicate, -1 otherwise.</returns>
    public static int FindIndex<T>(this IList<T> list, Predicate<T> predicate)
    {
        for (var i = 0; i < list.Count; i++)
        {
            if (predicate(list[i]))
            {
                return i;
            }
        }

        return -1;
    }
    #endregion

    #region Comparisons
    /// <summary>
    ///     Checks if the provided string appears in the original string in a case-insensitive manner.
    /// </summary>
    /// <param name="source">The original string.</param>
    /// <param name="toCheck">The string to look for in the original string.</param>
    /// <returns>True if the provided string appears, false otherwise.</returns>
    public static bool CaseInsensitiveContains(this string? source, string toCheck)
    {
        return source?.IndexOf(toCheck, StringComparison.InvariantCultureIgnoreCase) >= 0;
    }

    /// <summary>
    ///     Checks if the difference between the double and the value provided is within the provided tolerance.
    /// </summary>
    /// <param name="self">The number itself.</param>
    /// <param name="comparisonValue">The value to which it is being compared.</param>
    /// <param name="tolerance">The maximum allowable difference between the two values.</param>
    /// <returns>True if the difference in values is within tolerance, false otherwise.</returns>
    public static bool IsApproximatelyEqual(this double self, double comparisonValue,
        double tolerance = DEFAULT_APPROXIMATELY_EQUAL_TOLERANCE)
    {
        return Math.Abs(self - comparisonValue) <= tolerance;
    }
    #endregion

    #region String
    /// <summary>
    ///     Reverses the provided string.
    /// </summary>
    /// <param name="stringValue">The string to reverse.</param>
    /// <returns>The reversed string.</returns>
    public static string? ReverseString(string? stringValue)
    {
        if (stringValue is null)
        {
            return null;
        }

        var charArray = stringValue.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
    #endregion

    #region Enums
    /// <summary>
    ///     Gets the human-readable representation of the <see cref="StudyPlan.AcademicTerm" />.
    /// </summary>
    /// <param name="term">The <see cref="StudyPlan.AcademicTerm" /> to stringify.</param>
    /// <returns>A human-readable representation of the <see cref="StudyPlan.AcademicTerm" />.</returns>
    public static string GetAcademicTermString(this StudyPlan.AcademicTerm term)
    {
        return ReverseString(term.ToString()) ?? string.Empty;
    }

    /// <summary>
    ///     Gets the human-readable representation of the <see cref="Student.ProgramType" />.
    /// </summary>
    /// <param name="programType">The <see cref="Student.ProgramType" /> to stringify.</param>
    /// <returns>A human-readable representation of the <see cref="Student.ProgramType" />.</returns>
    public static string GetProgramTypeString(this Student.ProgramType programType)
    {
        return programType switch
        {
            Student.ProgramType.Computer => "Computer Engineering",
            Student.ProgramType.Electrical => "Electrical Engineering",
            _ => throw new ArgumentOutOfRangeException(nameof(programType), programType, null)
        };
    }

    /// <summary>
    ///     Gets the human-readable representation of the <see cref="Student.StreamType" />.
    /// </summary>
    /// <param name="streamType">The <see cref="Student.StreamType" /> to stringify.</param>
    /// <returns>A human-readable representation of the <see cref="Student.StreamType" />.</returns>
    public static string GetStreamTypeString(this Student.StreamType streamType)
    {
        return streamType switch
        {
            Student.StreamType.Four => "4",
            Student.StreamType.Eight => "8",
            _ => throw new ArgumentOutOfRangeException(nameof(streamType), streamType, null)
        };
    }
    #endregion
}
