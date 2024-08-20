namespace CourseEquivalencyDesktop.Models;

/// <summary>
/// Model to hold information for a single University.
/// </summary>
public class University
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Url { get; set; }

    public override string ToString()
    {
        return $"University: Name - {Name}; URL - {Url}";
    }
}
