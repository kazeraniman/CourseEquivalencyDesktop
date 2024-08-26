using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CourseEquivalencyDesktop.ViewModels.General;
using CourseEquivalencyDesktop.Views.General;

namespace CourseEquivalencyDesktop.Services;

public class GenericDialogService
{
    public async Task<bool?> OpenGenericDialog(string titleText, string bodyText,
        string primaryButtonText, string? secondaryButtonText = null, bool isCloseable = true)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            throw new ApplicationException("Not a desktop application");
        }

        var topMostWindow = desktop.Windows[^1];
        if (topMostWindow == null)
        {
            throw new ApplicationException("No top-most window.");
        }

        var genericDialogWindowViewModel =
            new GenericDialogWindowViewModel(titleText, bodyText, primaryButtonText, secondaryButtonText);
        var genericDialogWindow = new GenericDialogWindow
        {
            DataContext = genericDialogWindowViewModel
        };

        genericDialogWindowViewModel.OnRequestCloseWindow += (_, args) =>
            genericDialogWindow.Close((args as GenericDialogWindowViewModel.GenericDialogEventArgs)?.WasPrimary);
        genericDialogWindow.Closing += (result, e) =>
        {
            if (!isCloseable && result is null)
            {
                e.Cancel = true;
            }
        };

        return await genericDialogWindow.ShowDialog<bool?>(topMostWindow);
    }
}
