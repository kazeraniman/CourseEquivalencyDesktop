using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace CourseEquivalencyDesktop.Utility;

public static class Utility
{
    /// <summary>
    /// Ensures that we are currently using Design Mode.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if we are running ouside of Design Mode.</exception>
    /// <remarks>
    /// This would be nicer to have as an attribute, but attribute interception appears to not work on constructors from my reading.
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
    /// Adds all the provided items to the provided collection.
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

    public static bool CaseInsensitiveContains(this string? source, string toCheck)
    {
        return source?.IndexOf(toCheck, StringComparison.InvariantCultureIgnoreCase) >= 0;
    }
}
