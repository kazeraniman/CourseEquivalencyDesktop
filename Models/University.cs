using System.Collections.Generic;

namespace CourseEquivalencyDesktop.Models;

public class University : ModelBase
{
    #region Fields
    private int id;
    private string name;
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

    public ICollection<Course> Courses { get; }
    #endregion
}
