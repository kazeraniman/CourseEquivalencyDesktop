using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace CourseEquivalencyDesktop.Services;

/// <summary>
///     Spawns different file dialogs.
/// </summary>
public class FileDialogService
{
    #region Constants
    public const string SQLITE_DATABASE_DEFAULT_EXTENSION = "sqlite";
    #endregion

    #region Types
    /// <summary>
    ///     File picker type for Sqlite database files.
    /// </summary>
    public static FilePickerFileType SqliteDatabaseFilePickerFileType { get; } = new("Database")
    {
        Patterns = ["*.sqlite"],
        AppleUniformTypeIdentifiers = ["public.sqlite3"],
        MimeTypes = ["application/x-sqlite3"]
    };
    #endregion

    #region Dialogs
    /// <summary>
    ///     Opens a file picker dialog.
    /// </summary>
    /// <param name="title">The title for the file dialog.</param>
    /// <param name="shouldAllowMultiple">True if multiple files should be pickable, false for a single file.</param>
    /// <param name="fileTypes">The allowable file types for selection.</param>
    /// <returns>A task wrapping a list of picked files. The list will be empty if no selection is made.</returns>
    /// <exception cref="ApplicationException">
    ///     Thrown if the application is not in a state which can currently provide a file
    ///     dialog.
    /// </exception>
    public Task<IReadOnlyList<IStorageFile>> OpenFileDialog(string title, bool shouldAllowMultiple,
        params FilePickerFileType[] fileTypes)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            throw new ApplicationException("Not a desktop application");
        }

        // TODO: Allow for the start folder to be passed

        if (desktop.MainWindow == null)
        {
            throw new ApplicationException("No main window.");
        }

        return desktop.MainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = shouldAllowMultiple,
            FileTypeFilter = fileTypes
        });
    }

    /// <summary>
    ///     Opens a file save dialog.
    /// </summary>
    /// <param name="title">The title for the file dialog.</param>
    /// <param name="suggestedFileName">The default name for the file to be saved.</param>
    /// <param name="defaultExtension">The default extension for the file to be saved.</param>
    /// <param name="fileTypes">The allowable file types for saving.</param>
    /// <returns>A task wrapping a file to which we can save. The file will be null if no selection is made.</returns>
    /// <exception cref="ApplicationException">
    ///     Thrown if the application is not in a state which can currently provide a file
    ///     dialog.
    /// </exception>
    public Task<IStorageFile?> SaveFileDialog(string title, string suggestedFileName, string defaultExtension,
        params FilePickerFileType[] fileTypes)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            throw new ApplicationException("Not a desktop application");
        }

        // TODO: Allow for the start folder to be passed

        if (desktop.MainWindow == null)
        {
            throw new ApplicationException("No main window.");
        }

        return desktop.MainWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            FileTypeChoices = fileTypes,
            DefaultExtension = defaultExtension,
            SuggestedFileName = suggestedFileName
        });
    }
    #endregion
}
