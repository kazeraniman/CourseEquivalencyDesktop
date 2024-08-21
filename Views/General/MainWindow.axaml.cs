using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.ViewModels.DatabaseSelectionWizard;
using CourseEquivalencyDesktop.ViewModels.Universities;
using CourseEquivalencyDesktop.Views.DatabaseSelectionWizard;
using CourseEquivalencyDesktop.Views.Universities;
using MainWindowViewModel = CourseEquivalencyDesktop.ViewModels.General.MainWindowViewModel;

namespace CourseEquivalencyDesktop.Views.General;

public partial class MainWindow : Window
{
    private IDisposable? createUniversityInteractionDisposable;
    private IDisposable? spawnDatabaseSelectionWizardInteractionDisposable;

    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        createUniversityInteractionDisposable?.Dispose();
        spawnDatabaseSelectionWizardInteractionDisposable?.Dispose();
        if (DataContext is MainWindowViewModel vm)
        {
            createUniversityInteractionDisposable = vm.CreateUniversityInteraction.RegisterHandler(SpawnCreateUniversityWindow);
            spawnDatabaseSelectionWizardInteractionDisposable = vm.SpawnDatabaseSelectionWizardInteraction.RegisterHandler(SpawnDatabaseSelectionWizardWindow);
        }

        base.OnDataContextChanged(e);
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (DataContext is MainWindowViewModel vm)
        {
            vm.InitializationCommand.Execute(vm);
        }
    }

    private Task<bool?> SpawnDatabaseSelectionWizardWindow(bool? _)
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

        return databaseSelectionWizardWindow.ShowDialog<bool?>(this);
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
