using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.DependencyInjection;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.DatabaseSelectionWizard;
using CourseEquivalencyDesktop.Views.DatabaseSelectionWizard;
using MainWindowViewModel = CourseEquivalencyDesktop.ViewModels.General.MainWindowViewModel;

namespace CourseEquivalencyDesktop.Views.General;

public partial class MainWindow : Window
{
    #region Constants
    private const string DATABASE_REQUIRED_TITLE = "Database Required";

    private const string DATABASE_REQUIRED_BODY =
        "A database must be set for the application to function. Please complete the setup.";
    #endregion

    #region Fields
    private IDisposable? spawnDatabaseSelectionWizardInteractionDisposable;
    private IDisposable? getWindowNotificationManagerInteractionDisposable;
    #endregion

    #region Constructors
    public MainWindow()
    {
        InitializeComponent();
    }
    #endregion

    #region Avalonia Life Cycle
    protected override void OnDataContextChanged(EventArgs e)
    {
        CleanDisposables();
        if (DataContext is MainWindowViewModel vm)
        {
            spawnDatabaseSelectionWizardInteractionDisposable =
                vm.SpawnDatabaseSelectionWizardInteraction.RegisterHandler(SpawnDatabaseSelectionWizardWindow);
            getWindowNotificationManagerInteractionDisposable =
                vm.GetWindowNotificationManagerInteraction.RegisterHandler(GetWindowNotificationManager);
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

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        CleanDisposables();
    }
    #endregion

    #region Interaction Handlers
    private async Task<bool?> SpawnDatabaseSelectionWizardWindow(bool? _)
    {
        var databaseSelectionWizardViewModel = Ioc.Default.GetRequiredService<DatabaseSelectionWizardViewModel>();
        var genericDialogService = Ioc.Default.GetRequiredService<GenericDialogService>();
        var databaseSelectionWizardWindow = new DatabaseSelectionWizardWindow
        {
            DataContext = databaseSelectionWizardViewModel
        };

        databaseSelectionWizardViewModel.OnRequestCloseWindow += (_, _) => databaseSelectionWizardWindow.Close();
        databaseSelectionWizardWindow.Closing += async (_, e) =>
        {
            if (string.IsNullOrEmpty(Ioc.Default.GetRequiredService<UserSettingsService>().DatabaseFilePath))
            {
                await genericDialogService.OpenGenericDialog(DATABASE_REQUIRED_TITLE, DATABASE_REQUIRED_BODY,
                    Constants.GenericStrings.OKAY, isCloseable: false);

                // TODO: Maybe override the topbar so that it doesn't have close button at all and just add a cancel button myself that only works if there is a database set?
                // TODO: Maybe hide the main window while in this mode cause it won't have anything good to show yet
                e.Cancel = true;
            }
        };

        return await databaseSelectionWizardWindow.ShowDialog<bool?>(this);
    }

    private Task<WindowNotificationManager> GetWindowNotificationManager(bool? _)
    {
        return Task.FromResult(MainWindowNotificationManager);
    }
    #endregion

    #region Helpers
    private void CleanDisposables()
    {
        spawnDatabaseSelectionWizardInteractionDisposable?.Dispose();
        getWindowNotificationManagerInteractionDisposable?.Dispose();
    }
    #endregion
}
