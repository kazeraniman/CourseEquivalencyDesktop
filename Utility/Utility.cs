using System;
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
}
