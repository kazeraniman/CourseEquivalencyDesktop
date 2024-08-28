using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;

namespace CourseEquivalencyDesktop.ViewModels.Universities;

public partial class UniversitiesPageViewModel : ViewModelBase
{
    #region Constants
    private const string UNIVERSITY_DELETE_TITLE = "Delete University?";

    private const string UNIVERSITY_DELETE_BODY =
        "Are you sure you wish to delete \"{0}\"?\nThis action cannot be undone and will delete all associated entries.";

    private const string UNIVERSITY_FAILED_DELETE_TITLE = "University Deletion Failed";

    private const string UNIVERSITY_FAILED_DELETE_BODY =
        "An error occurred and the university could not be deleted.";
    #endregion

    #region Fields
    public readonly Interaction<University?, University?> CreateOrEditUniversityInteraction = new();

    private readonly ObservableCollection<University> universities = [];

    private readonly DatabaseService databaseService;
    private readonly UserSettingsService userSettingsService;
    private readonly GenericDialogService genericDialogService;
    #endregion

    #region Properties
    public DataGridCollectionView UniversitiesCollectionView { get; init; }

    #region Observable Properties
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    private string searchText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    private int currentHumanReadablePageIndex = 1;
    #endregion
    #endregion

    #region Constructors
    public UniversitiesPageViewModel()
    {
        Utility.Utility.AssertDesignMode();

        databaseService = new DatabaseService();
        userSettingsService = new UserSettingsService();
        genericDialogService = new GenericDialogService();
        UniversitiesCollectionView = new DataGridCollectionView(universities);
    }

    public UniversitiesPageViewModel(DatabaseService databaseService, UserSettingsService userSettingsService,
        GenericDialogService genericDialogService)
    {
        this.databaseService = databaseService;
        this.userSettingsService = userSettingsService;
        this.genericDialogService = genericDialogService;

        UpdateUniversities();

        UniversitiesCollectionView = new DataGridCollectionView(universities)
        {
            Filter = Filter,
            PageSize = userSettingsService.DataGridPageSize
        };

        UniversitiesCollectionView.PageChanged += PageChangedHandler;
        universities.CollectionChanged += CollectionChangedHandler;
    }
    #endregion

    #region Handlers
    private void CollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e)
    {
        PreviousPageCommand.NotifyCanExecuteChanged();
        NextPageCommand.NotifyCanExecuteChanged();
    }

    private void PageChangedHandler(object? sender, EventArgs e)
    {
        CurrentHumanReadablePageIndex = UniversitiesCollectionView.PageIndex + 1;
    }

    partial void OnSearchTextChanged(string value)
    {
        // TODO: Debounce so this doesn't constantly happen
        UniversitiesCollectionView.Refresh();
        UniversitiesCollectionView.MoveToFirstPage();
    }

    private bool Filter(object arg)
    {
        if (arg is not University university)
        {
            return false;
        }

        return string.IsNullOrWhiteSpace(SearchText) || university.Name.CaseInsensitiveContains(SearchText);
    }
    #endregion

    #region Utility
    public void UpdateUniversities()
    {
        universities.Clear();
        universities.AddRange(databaseService.Universities);
    }

    private int GetPageCount()
    {
        return (int)Math.Ceiling(
            (double)UniversitiesCollectionView.TotalItemCount / UniversitiesCollectionView.PageSize);
    }
    #endregion

    #region Command Execution Checks
    private bool CanGoToNextPage()
    {
        return GetPageCount() > CurrentHumanReadablePageIndex;
    }

    private bool CanGoToPreviousPage()
    {
        return CurrentHumanReadablePageIndex > 1;
    }
    #endregion

    #region Commands
    [RelayCommand]
    private async Task CreateUniversity()
    {
        var universityToCreate = await CreateOrEditUniversityInteraction.HandleAsync(null);
        if (universityToCreate is not null)
        {
            universities.Add(universityToCreate);
        }
    }

    [RelayCommand]
    private async Task EditUniversity(University university)
    {
        await CreateOrEditUniversityInteraction.HandleAsync(university);
    }

    [RelayCommand]
    private async Task DeleteUniversity(University university)
    {
        var shouldDelete = await genericDialogService.OpenGenericDialog(UNIVERSITY_DELETE_TITLE,
            string.Format(UNIVERSITY_DELETE_BODY, university.Name), Constants.GenericStrings.DELETE,
            Constants.GenericStrings.CANCEL, primaryButtonThemeName: Constants.ResourceNames.RED_BUTTON);
        if (shouldDelete is null or false)
        {
            return;
        }

        databaseService.Universities.Remove(university);
        await databaseService.SaveChangesAsyncWithCallbacks(SaveChangesSuccessHandler, SaveChangesFailedHandler);

        void SaveChangesSuccessHandler()
        {
            universities.Remove(university);
        }

        void SaveChangesFailedHandler()
        {
            _ = genericDialogService.OpenGenericDialog(UNIVERSITY_FAILED_DELETE_TITLE,
                UNIVERSITY_FAILED_DELETE_BODY, Constants.GenericStrings.OKAY);
        }
    }

    [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
    private void NextPage()
    {
        UniversitiesCollectionView.MoveToNextPage();
    }

    [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
    private void PreviousPage()
    {
        UniversitiesCollectionView.MoveToPreviousPage();
    }
    #endregion
}
