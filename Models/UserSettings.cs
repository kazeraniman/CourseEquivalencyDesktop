namespace CourseEquivalencyDesktop.Models;

public class UserSettings
{
    #region Properties
    public string? DatabaseFilePath { get; set; }
    public int DataGridPageSize { get; set; } = 10;
    public double SearchDebounceSeconds { get; set; } = 0.5;
    public string? UserFullName { get; set; }
    public string? UserDepartment { get; set; }
    public string? UserEmail { get; set; }
    #endregion

    #region Constructors
    public UserSettings()
    {
    }

    public UserSettings(string? databaseFilePath, int dataGridPageSize, double searchDebounceSeconds,
        string? userFullName, string? userDepartment, string? userEmail)
    {
        DatabaseFilePath = databaseFilePath;
        DataGridPageSize = dataGridPageSize;
        SearchDebounceSeconds = searchDebounceSeconds;
        UserFullName = userFullName;
        UserDepartment = userDepartment;
        UserEmail = userEmail;
    }

    public UserSettings(UserSettings userSettings) : this(userSettings.DatabaseFilePath, userSettings.DataGridPageSize,
        userSettings.SearchDebounceSeconds, userSettings.UserFullName, userSettings.UserDepartment,
        userSettings.UserEmail)
    {
    }
    #endregion
}
