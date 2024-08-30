using System.Collections.Generic;

namespace CourseEquivalencyDesktop.Models;

public class University : ModelBase
{
    #region Fields
    private int id;
    // Handled by EF Core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private string name;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private string? url;
    #endregion

    #region Properties
    public int Id
    {
        get => id;
        set => SetField(ref id, value);
    }

    public string Name
    {
        get => name;
        set => SetField(ref name, value);
    }

    public string? Url
    {
        get => url;
        set => SetField(ref url, value);
    }

    // Handled by EF Core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // ReSharper disable once UnassignedGetOnlyAutoProperty
    // ReSharper disable once CollectionNeverUpdated.Global
    public ICollection<Course> Courses { get; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion
}
