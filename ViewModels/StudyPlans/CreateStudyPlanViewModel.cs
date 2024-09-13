using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.ViewModels.General;

namespace CourseEquivalencyDesktop.ViewModels.StudyPlans;

public partial class CreateStudyPlanViewModel : BaseCreateOrEditViewModel<StudyPlan>
{
    #region Constants
    private const string CREATE_TEXT = "Create Study Plan";
    private const string DESTINATION_UNIVERSITY_MUST_BE_DIFFERENT_TITLE = "Invalid Destination University";

    private const string DESTINATION_UNIVERSITY_MUST_BE_DIFFERENT_BODY =
        "The destination university must be different than the student's home university.";
    #endregion

    #region Properties
    protected override string FailedSaveTitle => "Study Plan Changes Failed";
    protected override string FailedSaveBody => "An error occurred and the study plan changes could not be made.";

    #region Observable Properties
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateOrEditCommand))]
    [Required]
    private Student? student;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateOrEditCommand))]
    [Required]
    private University? destinationUniversity;

    [ObservableProperty]
    private Student[] students;

    [ObservableProperty]
    private University[] universities;
    #endregion
    #endregion

    #region Constructors
    public CreateStudyPlanViewModel()
    {
        WindowAndButtonText = CREATE_TEXT;
        Students = [];
        Universities = [];
    }

    public CreateStudyPlanViewModel(StudyPlan? studyPlan, DatabaseService databaseService) : base(studyPlan,
        databaseService)
    {
        if (studyPlan is not null)
        {
            throw new UnreachableException("Study plan which is not null should not be creatable.");
        }

        WindowAndButtonText = CREATE_TEXT;

        Universities = databaseService.Universities
            .OrderBy(uni => uni.Name)
            .ToArray();

        Students = databaseService.Students
            .OrderBy(stu => stu.Name)
            .ThenBy(stu => stu.StudentId)
            .ToArray();

        // Ensure the button is disabled if invalid but don't trigger errors as they haven't performed any actions yet
        CreateOrEditCommand.NotifyCanExecuteChanged();
    }
    #endregion

    #region Handlers
    partial void OnStudentChanged(Student? value)
    {
        ValidateProperty(value, nameof(Student));
    }

    partial void OnDestinationUniversityChanged(University? value)
    {
        ValidateProperty(value, nameof(DestinationUniversity));
    }
    #endregion

    #region Command Execution Checks
    protected override bool CanCreateOrEdit()
    {
        return base.CanCreateOrEdit() && Student is not null && DestinationUniversity is not null;
    }
    #endregion

    #region Commands
    protected override async Task CreateOrEditInherited()
    {
        if (Student!.University.Id == DestinationUniversity!.Id)
        {
            ShowNotification(DESTINATION_UNIVERSITY_MUST_BE_DIFFERENT_TITLE,
                DESTINATION_UNIVERSITY_MUST_BE_DIFFERENT_BODY, NotificationType.Error);
            return;
        }

        var entityEntry = await DatabaseService.AddAsync(new StudyPlan
        {
            Student = Student,
            DestinationUniversity = DestinationUniversity,
            UpdatedAt = DateTime.Now,
            Status = StudyPlanStatus.Pending,
            Year = DateTime.Now.Year,
            Seasonal = DateTime.Now.Month switch
            {
                >= 1 and <= 4 => StudyPlan.SeasonalTerm.Spring,
                >= 5 and <= 8 => StudyPlan.SeasonalTerm.Fall,
                >= 9 and <= 12 => StudyPlan.SeasonalTerm.Winter,
                _ => throw new UnreachableException("Month provided is invalid.")
            },
            Academic = StudyPlan.AcademicTerm.A3,
            LastCompletedAcademicTerm = StudyPlan.AcademicTerm.B2
        });
        await SaveChanges(entityEntry.Entity);
    }
    #endregion
}
