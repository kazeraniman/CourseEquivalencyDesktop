namespace CourseEquivalencyDesktop.Models;

public class UserSettings
{
    #region Properties
    public string? DatabaseFilePath { get; set; }
    public int DataGridPageSize { get; set; } = 10;
    #endregion
}
