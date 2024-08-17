using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
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

        SpawnDatabaseSelectionWizardWindow();
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
            // TODO: Prevent closing if there is no database location in the settings
        };


        databaseSelectionWizardWindow.ShowDialog(this);
    }
}
