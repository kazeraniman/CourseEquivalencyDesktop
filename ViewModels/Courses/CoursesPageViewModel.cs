using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;

namespace CourseEquivalencyDesktop.ViewModels.Courses;

public partial class CoursesPageViewModel : ViewModelBase
{
    #region Constants
    private const string COURSE_DELETE_TITLE = "Delete University?";

    private const string COURSE_DELETE_BODY =
        "Are you sure you wish to delete \"{0}\"?\nThis action cannot be undone and will delete all associated entries.";

    private const string COURSE_FAILED_DELETE_TITLE = "Course Deletion Failed";

    private const string COURSE_FAILED_DELETE_BODY =
        "An error occurred and the course could not be deleted.";
    #endregion

    #region Fields
    public readonly Interaction<Course?, Course?> CreateOrEditCourseInteraction = new();

    private readonly ObservableCollection<Course> courses = [];

    private readonly DatabaseService databaseService;
    private readonly UserSettingsService userSettingsService;
    private readonly GenericDialogService genericDialogService;
    #endregion

    #region Properties
    public DataGridCollectionView CoursesCollectionView { get; init; }

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
    public CoursesPageViewModel()
    {
        Utility.Utility.AssertDesignMode();

        databaseService = new DatabaseService();
        userSettingsService = new UserSettingsService();
        genericDialogService = new GenericDialogService();
        CoursesCollectionView = new DataGridCollectionView(courses);
    }

    public CoursesPageViewModel(DatabaseService databaseService, UserSettingsService userSettingsService,
        GenericDialogService genericDialogService)
    {
        this.databaseService = databaseService;
        this.userSettingsService = userSettingsService;
        this.genericDialogService = genericDialogService;

        CoursesCollectionView = new DataGridCollectionView(courses)
        {
            Filter = Filter,
            PageSize = userSettingsService.DataGridPageSize
        };

        CoursesCollectionView.PageChanged += PageChangedHandler;
        courses.CollectionChanged += CollectionChangedHandler;
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
        CurrentHumanReadablePageIndex = CoursesCollectionView.PageIndex + 1;
    }

    partial void OnSearchTextChanged(string value)
    {
        // TODO: Debounce so this doesn't constantly happen
        CoursesCollectionView.Refresh();
        CoursesCollectionView.MoveToFirstPage();
    }

    private bool Filter(object arg)
    {
        if (arg is not Course course)
        {
            return false;
        }

        return string.IsNullOrWhiteSpace(SearchText) || course.Name.CaseInsensitiveContains(SearchText) ||
               course.University.Name.CaseInsensitiveContains(SearchText) ||
               course.CourseId.CaseInsensitiveContains(SearchText);
    }
    #endregion

    #region Utility
    public void UpdateCourses()
    {
        courses.Clear();
        courses.AddRange(databaseService.Courses);
    }

    private int GetPageCount()
    {
        return (int)Math.Ceiling(
            (double)CoursesCollectionView.TotalItemCount / CoursesCollectionView.PageSize);
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

    private bool CanCreateCourse()
    {
        return databaseService.Universities.Any();
    }
    #endregion

    #region Commands
    [RelayCommand(CanExecute = nameof(CanCreateCourse))]
    private async Task CreateCourse()
    {
        var courseToCreate = await CreateOrEditCourseInteraction.HandleAsync(null);
        if (courseToCreate is not null)
        {
            courses.Add(courseToCreate);
        }
    }

    [RelayCommand]
    private async Task EditCourse(Course course)
    {
        var editedCourse = await CreateOrEditCourseInteraction.HandleAsync(course);
        if (editedCourse is not null)
        {
            CoursesCollectionView.Refresh();
        }
    }

    [RelayCommand]
    private async Task DeleteCourse(Course course)
    {
        var shouldDelete = await genericDialogService.OpenGenericDialog(COURSE_DELETE_TITLE,
            string.Format(COURSE_DELETE_BODY, course.Name), Constants.GenericStrings.DELETE,
            Constants.GenericStrings.CANCEL, primaryButtonThemeName: Constants.ResourceNames.RED_BUTTON);
        if (shouldDelete is null or false)
        {
            return;
        }

        databaseService.Courses.Remove(course);
        await databaseService.SaveChangesAsyncWithCallbacks(SaveChangesSuccessHandler, SaveChangesFailedHandler);

        void SaveChangesSuccessHandler()
        {
            courses.Remove(course);
        }

        void SaveChangesFailedHandler()
        {
            _ = genericDialogService.OpenGenericDialog(COURSE_FAILED_DELETE_TITLE,
                COURSE_FAILED_DELETE_BODY, Constants.GenericStrings.OKAY);
        }
    }

    [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
    private void NextPage()
    {
        CoursesCollectionView.MoveToNextPage();
    }

    [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
    private void PreviousPage()
    {
        CoursesCollectionView.MoveToPreviousPage();
    }
    #endregion
}
