namespace CourseEquivalencyDesktop.Models;

public class Course
{
    #region Properties
    public int Id { get; set; }
    public string Name { get; set; }
    public string CourseId { get; set; }
    public int UniversityId { get; set; }
    public University University { get; set; }
    public string? Url { get; set; }
    public string? Description { get; set; }
    #endregion
}
