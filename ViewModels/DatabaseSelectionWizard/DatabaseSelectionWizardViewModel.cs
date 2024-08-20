using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Services;

namespace CourseEquivalencyDesktop.ViewModels.DatabaseSelectionWizard;

public enum DatabaseSelectionOptions
{
    CreateNew,
    OpenExisting
}

public partial class DatabaseSelectionWizardViewModel : ViewModelBase
{
    private const string OPEN_EXISTING_DIALOG_TITLE = "Open Database";
    private const string CREATE_DIALOG_TITLE = "Create Database";
    private const string DEFAULT_DATABASE_NAME = "ExCourseEquivalencyDatabase";

    private readonly DatabaseSelectionWizardInitialPageViewModel databaseSelectionWizardInitialPageViewModel = new();
    private readonly DatabaseSelectionWizardCreatePageViewModel databaseSelectionWizardCreatePageViewModel = new();
    private readonly DatabaseSelectionWizardOpenPageViewModel databaseSelectionWizardOpenPageViewModel = new();
    private readonly DatabaseSelectionWizardFinalizationPageViewModel databaseSelectionWizardFinalizationPageViewModel = new();

    private readonly FileDialogService fileDialogService;

    [ObservableProperty]
    private DatabaseSelectionOptions databaseSelectionOption;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NavigateNextPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(CompleteWizardCommand))]
    private string? existingDatabaseFilePath;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NavigateNextPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(CompleteWizardCommand))]
    private string? newDatabaseFilePath;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NavigateNextPageCommand))]
    private IDatabaseSelectionWizardPageViewModel currentPage;

    [ObservableProperty]
    private bool isNextPageButtonShown;

    [ObservableProperty]
    private bool isPreviousPageButtonShown;

    [ObservableProperty]
    private bool isDoneButtonShown;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CompleteWizardCommand))]
    [NotifyCanExecuteChangedFor(nameof(NavigatePreviousPageCommand))]
    private bool isFinalizing;

    public event EventHandler OnRequestCloseWindow;

    public DatabaseSelectionWizardViewModel()
    {
        Utility.Utility.AssertDesignMode();

        CurrentPage = databaseSelectionWizardInitialPageViewModel;
        fileDialogService = new FileDialogService();
    }

    public DatabaseSelectionWizardViewModel(FileDialogService fileDialogService)
    {
        CurrentPage = databaseSelectionWizardInitialPageViewModel;
        this.fileDialogService = fileDialogService;
    }

    // ReSharper disable once UnusedParameterInPartialMethod
    partial void OnCurrentPageChanged(IDatabaseSelectionWizardPageViewModel value)
    {
        IsNextPageButtonShown = GetNextPage() is not null;
        IsPreviousPageButtonShown = GetPreviousPage() is not null;
        IsDoneButtonShown = CurrentPage is DatabaseSelectionWizardFinalizationPageViewModel;
    }

    private bool CanNavigatePrevious()
    {
        return !IsFinalizing;
    }

    private bool CanNavigateNext()
    {
        return CurrentPage switch
        {
            DatabaseSelectionWizardCreatePageViewModel => !string.IsNullOrEmpty(NewDatabaseFilePath),
            DatabaseSelectionWizardFinalizationPageViewModel => false,
            DatabaseSelectionWizardInitialPageViewModel => true,
            DatabaseSelectionWizardOpenPageViewModel => !string.IsNullOrEmpty(ExistingDatabaseFilePath),
            _ => throw new ArgumentOutOfRangeException(nameof(CurrentPage))
        };
    }

    private bool CanFinish()
    {
        return !IsFinalizing && ((DatabaseSelectionOption == DatabaseSelectionOptions.CreateNew && !string.IsNullOrEmpty(NewDatabaseFilePath)) ||
               (DatabaseSelectionOption == DatabaseSelectionOptions.OpenExisting && !string.IsNullOrEmpty(ExistingDatabaseFilePath)));
    }

    private IDatabaseSelectionWizardPageViewModel? GetPreviousPage()
    {
        return CurrentPage switch
        {
            DatabaseSelectionWizardFinalizationPageViewModel => DatabaseSelectionOption == DatabaseSelectionOptions.CreateNew
                ? databaseSelectionWizardCreatePageViewModel
                : databaseSelectionWizardOpenPageViewModel,
            DatabaseSelectionWizardCreatePageViewModel or DatabaseSelectionWizardOpenPageViewModel =>
                databaseSelectionWizardInitialPageViewModel,
            _ => null
        };
    }

    private IDatabaseSelectionWizardPageViewModel? GetNextPage()
    {
        return CurrentPage switch
        {
            DatabaseSelectionWizardInitialPageViewModel => DatabaseSelectionOption == DatabaseSelectionOptions.CreateNew
                ? databaseSelectionWizardCreatePageViewModel
                : databaseSelectionWizardOpenPageViewModel,
            DatabaseSelectionWizardCreatePageViewModel or DatabaseSelectionWizardOpenPageViewModel =>
                databaseSelectionWizardFinalizationPageViewModel,
            _ => null
        };
    }

    [RelayCommand(CanExecute = nameof(CanNavigatePrevious))]
    private void NavigatePreviousPage()
    {
        var previousPage = GetPreviousPage();
        if (previousPage is not null)
        {
            CurrentPage = previousPage;
        }
    }

    [RelayCommand(CanExecute = nameof(CanNavigateNext))]
    private void NavigateNextPage()
    {
        var nextPage = GetNextPage();
        if (nextPage is not null)
        {
            CurrentPage = nextPage;
        }
    }

    [RelayCommand]
    private async Task SelectExistingDatabase()
    {
        var databaseFiles = await fileDialogService.OpenFileDialog(OPEN_EXISTING_DIALOG_TITLE, false, FileDialogService.SqliteDatabaseFilePickerFileType);
        if (databaseFiles.Count == 0)
        {
            return;
        }

        ExistingDatabaseFilePath = databaseFiles[0].Path.ToString();
    }

    [RelayCommand]
    private async Task SelectCreatedDatabase()
    {
        var databaseFile = await fileDialogService.SaveFileDialog(CREATE_DIALOG_TITLE, DEFAULT_DATABASE_NAME, FileDialogService.SQLITE_DATABASE_DEFAULT_EXTENSION, FileDialogService.SqliteDatabaseFilePickerFileType);
        if (databaseFile is null)
        {
            return;
        }

        NewDatabaseFilePath = databaseFile.Path.ToString();
    }

    [RelayCommand(CanExecute = nameof(CanFinish))]
    private async Task CompleteWizard()
    {
        // TODO: Actually complete the wizard stuff.
        IsFinalizing = true;
        var userSettingsService = Ioc.Default.GetRequiredService<UserSettingsService>();
        await userSettingsService.SetDatabaseFilePath(DatabaseSelectionOption == DatabaseSelectionOptions.CreateNew ? NewDatabaseFilePath : ExistingDatabaseFilePath);

        IsFinalizing = false;
        OnRequestCloseWindow(this, EventArgs.Empty);
    }
}
