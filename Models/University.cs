namespace CourseEquivalencyDesktop.Models;

public class University
{
    #region Properties
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Url { get; set; }
    #endregion

    #region Object
    public override string ToString()
    {
        return $"University: Id - {Id}; Name - {Name}; URL - {Url}";
    }
    #endregion
}
