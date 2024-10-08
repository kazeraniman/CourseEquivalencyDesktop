﻿using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CourseEquivalencyDesktop.Models;

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
    public UserSettings UserSettings => new(userSettings);
    public string? DatabaseFilePath => userSettings.DatabaseFilePath;
    public int DataGridPageSize => userSettings.DataGridPageSize;
    public double SearchDebounceSeconds => userSettings.SearchDebounceSeconds;
    public TimeSpan SearchDebounceSecondsTimeSpan => TimeSpan.FromSeconds(userSettings.SearchDebounceSeconds);
    public string? UserFullName => userSettings.UserFullName;
    public string? UserDepartment => userSettings.UserDepartment;
    public string? UserEmail => userSettings.UserEmail;
    public string? CreditTransferMemoTemplateFilePath => userSettings.CreditTransferMemoTemplateFilePath;
    public string? ProposedStudyPlanTemplateFilePath => userSettings.ProposedStudyPlanTemplateFilePath;
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
    ///     Set all the settings at once.
    /// </summary>
    /// <param name="newUserSettings">The new values of the user settings.</param>
    public async Task SetAllUserSettings(UserSettings newUserSettings)
    {
        if (userSettings == newUserSettings)
        {
            return;
        }

        userSettings = newUserSettings;
        await SaveSettings();
    }

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
    #endregion
}
