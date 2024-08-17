using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.ViewModels.DatabaseSelectionWizard;
using CourseEquivalencyDesktop.ViewModels.Universities;
using CourseEquivalencyDesktop.Views.DatabaseSelectionWizard;
using CourseEquivalencyDesktop.Views.Universities;

namespace CourseEquivalencyDesktop.Views.General;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        // TODO: Need to check if the DB connected too as an additional check after this
        if (string.IsNullOrEmpty(Ioc.Default.GetService<UserSettingsService>()?.DatabaseFilePath))
        {
            SpawnDatabaseSelectionWizardWindow();
        }
    }

    /// <summary>
    /// Create a window to allow for the creation of a new university.
    /// </summary>
    [RelayCommand]
    private void SpawnCreateUniversityWindow()
    {
        var createUniversityViewModel = new CreateUniversityViewModel();
        var createUniversityWindow = new CreateUniversityWindow
        {
            DataContext = createUniversityViewModel
        };

        createUniversityViewModel.OnRequestCloseWindow += (_, _) => createUniversityWindow.Close();
        createUniversityWindow.ShowDialog(this);
    }

    private void SpawnDatabaseSelectionWizardWindow()
    {
        var databaseSelectionWizardViewModel = Ioc.Default.GetService<DatabaseSelectionWizardViewModel>();
        var databaseSelectionWizardWindow = new DatabaseSelectionWizardWindow
        {
            DataContext = databaseSelectionWizardViewModel
        };

        databaseSelectionWizardWindow.Closing += (_, e) =>
        {
            if (string.IsNullOrEmpty(Ioc.Default.GetService<UserSettingsService>()?.DatabaseFilePath))
            {
                // TODO: Maybe show a dialog to tell them why they can't close it
                // TODO: Maybe override the topbar so that it doesn't have close button at all and just add a cancel button myself that only works if there is a database set?
                // TODO: Maybe hide the main window while in this mode cause it won't have anything good to show yet
                e.Cancel = true;
            }
        };


        databaseSelectionWizardWindow.ShowDialog(this);
    }
}
