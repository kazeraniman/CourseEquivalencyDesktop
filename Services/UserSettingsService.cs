using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CourseEquivalencyDesktop.Models;

namespace CourseEquivalencyDesktop.Services;

public class UserSettingsService
{
    private const string APP_DATA_FOLDER_NAME = "CourseEquivalencyDesktop";
    private const string USER_SETTINGS_FILE_NAME = "Settings.json";

    private static readonly string userSettingsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        APP_DATA_FOLDER_NAME);
    private static readonly string userSettingsFilePath = Path.Combine(userSettingsFolderPath,
        USER_SETTINGS_FILE_NAME);

    private UserSettings userSettings = new();

    public string? DatabaseFilePath => userSettings.DatabaseFilePath;
    public int DataGridPageSize => userSettings.DataGridPageSize;

    public async Task LoadSettings()
    {
        if (!File.Exists(userSettingsFilePath))
        {
            return;
        }

        await using var fileStream = File.OpenRead(userSettingsFilePath);
        userSettings = await JsonSerializer.DeserializeAsync<UserSettings>(fileStream) ?? userSettings;
    }

    private async Task SaveSettings()
    {
        Directory.CreateDirectory(userSettingsFolderPath);
        await using var createStream = File.Create(userSettingsFilePath);
        await JsonSerializer.SerializeAsync(createStream, userSettings);
    }

    public async Task SetDatabaseFilePath(string? databaseFilePath)
    {
        userSettings.DatabaseFilePath = databaseFilePath;
        await SaveSettings();
    }

    public async Task SetDataGridPageSize(int dataGridPageSize)
    {
        userSettings.DataGridPageSize = dataGridPageSize;
        await SaveSettings();
    }
}
