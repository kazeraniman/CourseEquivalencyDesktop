using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.DependencyInjection;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.ViewModels;
using CourseEquivalencyDesktop.ViewModels.DatabaseSelectionWizard;
using CourseEquivalencyDesktop.ViewModels.Universities;
using CourseEquivalencyDesktop.Views.DatabaseSelectionWizard;
using CourseEquivalencyDesktop.Views.Universities;
using Microsoft.EntityFrameworkCore;

namespace CourseEquivalencyDesktop.Views.General;

public partial class MainWindow : Window
{
    private IDisposable? createUniversityInteractionDisposable;

    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        createUniversityInteractionDisposable?.Dispose();
        if (DataContext is MainWindowViewModel vm)
        {
            createUniversityInteractionDisposable = vm.CreateUniversityInteraction.RegisterHandler(SpawnCreateUniversityWindow);
        }

        base.OnDataContextChanged(e);
    }

    protected override async void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is not MainWindowViewModel mainWindowViewModel)
        {
            return;
        }

        mainWindowViewModel.IsLoaded = false;

        if (string.IsNullOrEmpty(Ioc.Default.GetRequiredService<UserSettingsService>().DatabaseFilePath))
        {
            await SpawnDatabaseSelectionWizardWindow();
        }

        await Ioc.Default.GetRequiredService<DatabaseService>().Database.MigrateAsync();

        mainWindowViewModel.IsLoaded = true;
    }

    private Task SpawnDatabaseSelectionWizardWindow()
    {
        var databaseSelectionWizardViewModel = Ioc.Default.GetRequiredService<DatabaseSelectionWizardViewModel>();
        var databaseSelectionWizardWindow = new DatabaseSelectionWizardWindow
        {
            DataContext = databaseSelectionWizardViewModel
        };

        databaseSelectionWizardViewModel.OnRequestCloseWindow += (_, _) => databaseSelectionWizardWindow.Close();
        databaseSelectionWizardWindow.Closing += (_, e) =>
        {
            if (string.IsNullOrEmpty(Ioc.Default.GetRequiredService<UserSettingsService>().DatabaseFilePath))
            {
                // TODO: Maybe show a dialog to tell them why they can't close it
                // TODO: Maybe override the topbar so that it doesn't have close button at all and just add a cancel button myself that only works if there is a database set?
                // TODO: Maybe hide the main window while in this mode cause it won't have anything good to show yet
                e.Cancel = true;
            }
        };

        return databaseSelectionWizardWindow.ShowDialog(this);
    }

    private async Task<University?> SpawnCreateUniversityWindow(int? id)
    {
        var createUniversityViewModel = new CreateUniversityViewModel();
        var createUniversityWindow = new CreateUniversityWindow
        {
            DataContext = createUniversityViewModel
        };

        createUniversityViewModel.OnRequestCloseWindow += (_, args) => createUniversityWindow.Close((args as CreateUniversityViewModel.CreateUniversityEventArgs)?.University);
        return await createUniversityWindow.ShowDialog<University>(this);
    }
}
