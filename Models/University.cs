using System.Collections.Generic;

namespace CourseEquivalencyDesktop.Models;

public class University
{
    #region Properties
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Url { get; set; }
    public ICollection<Course> Courses { get; }
    #endregion
}
