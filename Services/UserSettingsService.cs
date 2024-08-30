using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Utility;

namespace CourseEquivalencyDesktop.Services;

/// <summary>
///     Manages user settings which can be persisted between sessions.
/// </summary>
public class UserSettingsService
{
    #region Constants / Static Readonly
    private const string APP_DATA_FOLDER_NAME = "ExCourseEquivalency";
    private const string USER_SETTINGS_FILE_NAME = "Settings.json";

    private static readonly string userSettingsFolderPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        APP_DATA_FOLDER_NAME);

    private static readonly string userSettingsFilePath = Path.Combine(userSettingsFolderPath,
        USER_SETTINGS_FILE_NAME);

    private static readonly JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerOptions.Default)
    {
        WriteIndented = true
    };
    #endregion

    #region Fields
    private UserSettings userSettings = new();
    #endregion

    #region Properties
    public string? DatabaseFilePath => userSettings.DatabaseFilePath;
    public int DataGridPageSize => userSettings.DataGridPageSize;
    public double SearchDebounceSeconds => userSettings.SearchDebounceSeconds;
    public TimeSpan SearchDebounceSecondsTimeSpan => TimeSpan.FromSeconds(userSettings.SearchDebounceSeconds);
    #endregion

    #region File Management
    /// <summary>
    ///     Load the settings from the save file.
    /// </summary>
    public async Task LoadSettings()
    {
        if (!File.Exists(userSettingsFilePath))
        {
            return;
        }

        await using var fileStream = File.OpenRead(userSettingsFilePath);
        userSettings = await JsonSerializer.DeserializeAsync<UserSettings>(fileStream) ?? userSettings;
    }

    /// <summary>
    ///     Write the settings to the save file.
    /// </summary>
    private async Task SaveSettings()
    {
        Directory.CreateDirectory(userSettingsFolderPath);
        await using var createStream = File.Create(userSettingsFilePath);
        await JsonSerializer.SerializeAsync(createStream, userSettings, jsonSerializerOptions);
    }
    #endregion

    #region Setters
    /// <summary>
    ///     Set the database file path to memory and file.
    /// </summary>
    /// <param name="databaseFilePath">The new value of the database file path.</param>
    public async Task SetDatabaseFilePath(string? databaseFilePath)
    {
        if (databaseFilePath == DatabaseFilePath)
        {
            return;
        }

        userSettings.DatabaseFilePath = databaseFilePath;
        await SaveSettings();
    }

    /// <summary>
    ///     Set the data grid page size in memory and file.
    /// </summary>
    /// <param name="dataGridPageSize">The new value of the data grid page size.</param>
    public async Task SetDataGridPageSize(int dataGridPageSize)
    {
        if (dataGridPageSize == DataGridPageSize)
        {
            return;
        }

        userSettings.DataGridPageSize = dataGridPageSize;
        await SaveSettings();
    }

    /// <summary>
    ///     Set the search debounce seconds in memory and file.
    /// </summary>
    /// <param name="searchDebounceSeconds">The new value of the search debounce seconds.</param>
    public async Task SetSearchDebounceSeconds(double searchDebounceSeconds)
    {
        if (searchDebounceSeconds.IsApproximatelyEqual(SearchDebounceSeconds))
        {
            return;
        }

        userSettings.SearchDebounceSeconds = searchDebounceSeconds;
        await SaveSettings();
    }
    #endregion
}
