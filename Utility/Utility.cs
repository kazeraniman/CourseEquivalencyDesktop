using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;

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

    #region Methods
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
}
