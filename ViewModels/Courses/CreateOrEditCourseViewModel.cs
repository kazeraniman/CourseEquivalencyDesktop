using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;
using Microsoft.EntityFrameworkCore;

namespace CourseEquivalencyDesktop.ViewModels.Courses;

public partial class CreateOrEditCourseViewModel : BaseCreateOrEditViewModel<Course>
{
    #region Constants
    private const string CREATE_TEXT = "Create Course";
    private const string EDIT_TEXT = "Edit Course";
    private const string COURSE_CODE_EXISTS_TITLE = "Course Exists";
    private const string COURSE_CODE_EXISTS_BODY = "A course with this course code already exists in this university.";
    private const string COURSE_EDITING_NOT_EXIST_TITLE = "Course Doesn't Exist";
    private const string COURSE_EDITING_NOT_EXIST_BODY = "The course you are trying to edit does not exist.";
    #endregion

    #region Properties
    protected override string FailedSaveTitle => "Course Changes Failed";
    protected override string FailedSaveBody => "An error occurred and the course changes could not be made.";

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
    private University[] universities;
    #endregion
    #endregion

    #region Constructors
    public CreateOrEditCourseViewModel()
    {
        WindowAndButtonText = CREATE_TEXT;
        Universities = [];
        University = new University();
    }

    public CreateOrEditCourseViewModel(Course? course, DatabaseService databaseService,
        GenericDialogService genericDialogService) : base(course, databaseService, genericDialogService)
    {
        WindowAndButtonText = IsCreate ? CREATE_TEXT : EDIT_TEXT;

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
    protected override bool CanCreateOrEdit()
    {
        return base.CanCreateOrEdit() && !(string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(CourseId));
    }
    #endregion

    #region Commands
    protected override async Task CreateOrEditInherited()
    {
        var preparedName = Name.Trim();
        var preparedUrl = Url?.Trim();
        var preparedCourseId = CourseId.Trim();
        var preparedDescription = Description?.Trim();
        if (IsCreate)
        {
            await Create();
        }
        else
        {
            await Update();
        }

        async Task Create()
        {
            var doesCourseIdExist = await DatabaseService.Courses.AnyAsync(cou =>
                cou.UniversityId == University.Id && cou.CourseId == preparedCourseId);
            if (doesCourseIdExist)
            {
                await GenericDialogService.OpenGenericDialog(COURSE_CODE_EXISTS_TITLE,
                    COURSE_CODE_EXISTS_BODY, Constants.GenericStrings.OKAY);
                return;
            }

            var entityEntry = await DatabaseService.AddAsync(new Course
            {
                University = University,
                Name = preparedName,
                CourseId = preparedCourseId,
                Url = preparedUrl,
                Description = preparedDescription
            });
            await SaveChanges(entityEntry.Entity);
        }

        async Task Update()
        {
            var editingCourse =
                await DatabaseService.Courses.FirstOrDefaultAsync(cou => cou.Id == Item!.Id);
            if (editingCourse is null)
            {
                await GenericDialogService.OpenGenericDialog(COURSE_EDITING_NOT_EXIST_TITLE,
                    COURSE_EDITING_NOT_EXIST_BODY, Constants.GenericStrings.OKAY);
                return;
            }

            var doesCourseIdExist = await DatabaseService.Courses.AnyAsync(cou =>
                cou.UniversityId == University.Id && cou.CourseId == preparedCourseId && cou.Id != editingCourse.Id);
            if (doesCourseIdExist)
            {
                await GenericDialogService.OpenGenericDialog(COURSE_CODE_EXISTS_TITLE,
                    COURSE_CODE_EXISTS_BODY, Constants.GenericStrings.OKAY);
                return;
            }

            editingCourse.University = University;
            editingCourse.Name = preparedName;
            editingCourse.CourseId = preparedCourseId;
            editingCourse.Url = preparedUrl;
            editingCourse.Description = preparedDescription;
            await SaveChanges(editingCourse);
        }
    }
    #endregion
}