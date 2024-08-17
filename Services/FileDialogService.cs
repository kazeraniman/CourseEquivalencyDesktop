using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
namespace CourseEquivalencyDesktop.Services;

public class FileDialogService
{
    public const string SQLITE_DATABASE_DEFAULT_EXTENSION = "sqlite";
    public static FilePickerFileType SqliteDatabaseFilePickerFileType { get; } = new("Database")
    {
        Patterns = new[] { "*.sqlite" },
        AppleUniformTypeIdentifiers = new[] { "public.sqlite3" },
        MimeTypes = new[] { "application/x-sqlite3" }
    };

    public Task<IReadOnlyList<IStorageFile>> OpenFileDialog(string title, bool shouldAllowMultiple, params FilePickerFileType[] fileTypes)
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
            FileTypeFilter = fileTypes,
        });
    }

    public Task<IStorageFile?> SaveFileDialog(string title, string suggestedFileName, string defaultExtension, params FilePickerFileType[] fileTypes)
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
}
