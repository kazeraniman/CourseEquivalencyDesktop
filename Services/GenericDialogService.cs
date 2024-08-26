using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;
using CourseEquivalencyDesktop.Views.General;

namespace CourseEquivalencyDesktop.Services;

public class GenericDialogService
{
    public async Task<bool?> OpenGenericDialog(string titleText, string bodyText,
        string primaryButtonText, string? secondaryButtonText = null, bool isCloseable = true,
        bool isSecondaryButtonCancel = true, string primaryButtonThemeName = Constants.ResourceNames.BLUE_BUTTON)
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
            new GenericDialogWindowViewModel(titleText, bodyText, primaryButtonText, secondaryButtonText,
                isCloseable, isSecondaryButtonCancel, primaryButtonThemeName);
        var genericDialogWindow = new GenericDialogWindow
        {
            DataContext = genericDialogWindowViewModel
        };

        genericDialogWindowViewModel.OnRequestCloseWindow += (_, args) =>
            genericDialogWindow.Close((args as GenericDialogWindowViewModel.GenericDialogEventArgs)?.WasPrimary);
        genericDialogWindow.Closing += (r, e) =>
        {
            if (!(isCloseable || e.IsProgrammatic))
            {
                e.Cancel = true;
            }
        };

        return await genericDialogWindow.ShowDialog<bool?>(topMostWindow);
    }
}
