using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;
using Microsoft.EntityFrameworkCore;

namespace CourseEquivalencyDesktop.ViewModels.Courses;

public partial class CreateOrEditCourseViewModel : ViewModelBase
{
    public class CreateOrEditCourseEventArgs : EventArgs
    {
        public Course? Course { get; init; }
    }

    #region Constants
    private const string CREATE_TEXT = "Create Course";
    private const string EDIT_TEXT = "Edit Course";
    private const string COURSE_CODE_EXISTS_TITLE = "Course Exists";
    private const string COURSE_CODE_EXISTS_BODY = "A course with this course code already exists in this university.";
    private const string COURSE_EDITING_NOT_EXIST_TITLE = "Course Doesn't Exist";
    private const string COURSE_EDITING_NOT_EXIST_BODY = "The course you are trying to edit does not exist.";
    private const string COURSE_FAILED_SAVE_TITLE = "Course Changes Failed";

    private const string COURSE_FAILED_SAVE_BODY =
        "An error occurred and the course changes could not be made.";
    #endregion

    #region Fields
    private readonly DatabaseService databaseService;
    private readonly GenericDialogService genericDialogService;

    private readonly Course? course;
    private readonly bool isCreate;
    #endregion

    #region Properties
    #region Observable Properties
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateOrEditCommand))]
    [Required(AllowEmptyStrings = false)]
    private string courseId = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateOrEditCommand))]
    [Required(AllowEmptyStrings = false)]
    private string name = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateOrEditCommand))]
    [Required]
    private University university;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateOrEditCommand))]
    [Url]
    private string? url;

    [ObservableProperty]
    private string? description;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateOrEditCommand), nameof(CancelCommand))]
    private bool isCreating;

    [ObservableProperty]
    private string windowAndButtonText;

    [ObservableProperty]
    private University[] universities;
    #endregion
    #endregion

    #region Events
    public event EventHandler? OnRequestCloseWindow;
    #endregion

    #region Constructors
    public CreateOrEditCourseViewModel()
    {
        Utility.Utility.AssertDesignMode();

        databaseService = new DatabaseService();
        genericDialogService = new GenericDialogService();
        WindowAndButtonText = CREATE_TEXT;
        universities = [];
        University = new University();
    }

    public CreateOrEditCourseViewModel(Course? course, DatabaseService databaseService,
        GenericDialogService genericDialogService)
    {
        this.databaseService = databaseService;
        this.genericDialogService = genericDialogService;
        this.course = course;
        isCreate = course is null;
        WindowAndButtonText = isCreate ? CREATE_TEXT : EDIT_TEXT;

        Name = course?.Name ?? string.Empty;
        CourseId = course?.CourseId ?? string.Empty;
        Url = course?.Url;
        Description = course?.Description;
        Universities = databaseService.Universities
            .OrderBy(uni => uni.Name)
            .ToArray();
        University = course?.University ?? Universities[0];

        // Ensure the button is disabled if invalid but don't trigger errors as they haven't performed any actions yet
        CreateOrEditCommand.NotifyCanExecuteChanged();
    }
    #endregion

    #region Handlers
    partial void OnNameChanged(string value)
    {
        ValidateProperty(value, nameof(Name));
    }

    partial void OnCourseIdChanged(string value)
    {
        ValidateProperty(value, nameof(CourseId));
    }

    partial void OnUrlChanged(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            ClearErrors(nameof(Url));
            return;
        }

        ValidateProperty(value, nameof(Url));
    }
    #endregion

    #region Command Execution Checks
    private bool CanCancel()
    {
        return !IsCreating;
    }

    private bool CanCreateOrEdit()
    {
        return !(IsCreating || HasErrors || string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(CourseId));
    }
    #endregion

    #region Commands
    [RelayCommand(CanExecute = nameof(CanCreateOrEdit))]
    private async Task CreateOrEdit()
    {
        Course modifiedCourse;
        var preparedName = Name.Trim();
        var preparedUrl = Url?.Trim();
        var preparedCourseId = CourseId.Trim();
        var preparedDescription = Description?.Trim();
        if (isCreate)
        {
            await Create();
        }
        else
        {
            await Update();
        }

        async Task Create()
        {
            var doesCourseIdExist = await databaseService.Courses.AnyAsync(cou =>
                cou.UniversityId == University.Id && cou.CourseId == preparedCourseId);
            if (doesCourseIdExist)
            {
                await genericDialogService.OpenGenericDialog(COURSE_CODE_EXISTS_TITLE,
                    COURSE_CODE_EXISTS_BODY, Constants.GenericStrings.OKAY);
                return;
            }

            var entityEntry = await databaseService.AddAsync(new Course
            {
                University = University,
                Name = preparedName,
                CourseId = preparedCourseId,
                Url = preparedUrl,
                Description = preparedDescription
            });
            modifiedCourse = entityEntry.Entity;
            await Save();
        }

        async Task Update()
        {
            var editingCourse =
                await databaseService.Courses.FirstOrDefaultAsync(cou => cou.Id == course!.Id);
            if (editingCourse is null)
            {
                await genericDialogService.OpenGenericDialog(COURSE_EDITING_NOT_EXIST_TITLE,
                    COURSE_EDITING_NOT_EXIST_BODY, Constants.GenericStrings.OKAY);
                return;
            }

            var doesCourseIdExist = await databaseService.Courses.AnyAsync(cou =>
                cou.UniversityId == University.Id && cou.CourseId == preparedCourseId && cou.Id != editingCourse.Id);
            if (doesCourseIdExist)
            {
                await genericDialogService.OpenGenericDialog(COURSE_CODE_EXISTS_TITLE,
                    COURSE_CODE_EXISTS_BODY, Constants.GenericStrings.OKAY);
                return;
            }

            editingCourse.University = University;
            editingCourse.Name = preparedName;
            editingCourse.CourseId = preparedCourseId;
            editingCourse.Url = preparedUrl;
            editingCourse.Description = preparedDescription;
            modifiedCourse = editingCourse;
            await Save();
        }

        async Task Save()
        {
            databaseService.SaveChangesFailed += SaveChangesFailedHandler;
            databaseService.SavedChanges += SaveChangesSuccessHandler;
            await databaseService.SaveChangesAsync();
        }

        void Unsubscribe()
        {
            databaseService.SaveChangesFailed -= SaveChangesFailedHandler;
            databaseService.SavedChanges -= SaveChangesSuccessHandler;
        }

        void SaveChangesFailedHandler(object? sender, SaveChangesFailedEventArgs e)
        {
            Unsubscribe();
            _ = genericDialogService.OpenGenericDialog(COURSE_FAILED_SAVE_TITLE,
                COURSE_FAILED_SAVE_BODY, Constants.GenericStrings.OKAY);
        }

        void SaveChangesSuccessHandler(object? sender, SavedChangesEventArgs e)
        {
            Unsubscribe();
            OnRequestCloseWindow?.Invoke(this, new CreateOrEditCourseEventArgs { Course = modifiedCourse });
        }
    }

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel()
    {
        OnRequestCloseWindow?.Invoke(this, EventArgs.Empty);
    }
    #endregion
}
