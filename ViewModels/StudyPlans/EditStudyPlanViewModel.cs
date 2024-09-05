using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;

namespace CourseEquivalencyDesktop.ViewModels.StudyPlans;

public partial class EditStudyPlanViewModel : BaseCreateOrEditViewModel<StudyPlan>
{
    #region Constants
    private const string EDIT_TEXT = "Edit Study Plan";
    private const string STUDY_PLAN_EDITING_NOT_EXIST_TITLE = "Study Plan Doesn't Exist";
    private const string STUDY_PLAN_EDITING_NOT_EXIST_BODY = "The study plan you are trying to edit does not exist.";
    #endregion

    #region Properties
    protected override string FailedSaveTitle => "Study Plan Changes Failed";
    protected override string FailedSaveBody => "An error occurred and the study plan changes could not be made.";

    #region Observable Properties
    [ObservableProperty]
    private Student student;

    [ObservableProperty]
    private University destinationUniversity;

    [ObservableProperty]
    private StudyPlanStatus studyPlanStatus;

    [ObservableProperty]
    [Required]
    private StudyPlan.AcademicTerm academicTerm;

    [ObservableProperty]
    [Required]
    private StudyPlan.SeasonalTerm seasonalTerm;

    [ObservableProperty]
    [Required]
    private int year;

    [ObservableProperty]
    private DateTime? dueDate;
    #endregion

    [ObservableProperty]
    private string? notes;

    [ObservableProperty]
    private StudyPlan.AcademicTerm[] academicTerms = Enum.GetValues<StudyPlan.AcademicTerm>();

    [ObservableProperty]
    private StudyPlan.SeasonalTerm[] seasonalTerms = Enum.GetValues<StudyPlan.SeasonalTerm>();
    #endregion

    #region Constructors
    public EditStudyPlanViewModel()
    {
        WindowAndButtonText = EDIT_TEXT;

        DestinationUniversity = new University
        {
            Name = "Test University"
        };
        Student = new Student
        {
            Name = "Test Student",
            StudentId = "12345678",
            University = DestinationUniversity
        };
    }

    public EditStudyPlanViewModel(StudyPlan? studyPlan, DatabaseService databaseService,
        GenericDialogService genericDialogService) : base(studyPlan, databaseService, genericDialogService)
    {
        if (studyPlan is null)
        {
            throw new UnreachableException("Study plan which is null should not be editable.");
        }

        WindowAndButtonText = EDIT_TEXT;

        Student = studyPlan.Student;
        DestinationUniversity = studyPlan.DestinationUniversity;
        StudyPlanStatus = studyPlan.Status;
        AcademicTerm = studyPlan.Academic;
        SeasonalTerm = studyPlan.Seasonal;
        Year = studyPlan.Year;
        DueDate = studyPlan.DueDate;
        Notes = studyPlan.Notes;

        // Ensure the button is disabled if invalid but don't trigger errors as they haven't performed any actions yet
        CreateOrEditCommand.NotifyCanExecuteChanged();
    }
    #endregion

    #region Commands
    protected override async Task CreateOrEditInherited()
    {
        var editingStudyPlan =
            await DatabaseService.StudyPlans.FindAsync(Item!.Id);
        if (editingStudyPlan is null)
        {
            await GenericDialogService.OpenGenericDialog(STUDY_PLAN_EDITING_NOT_EXIST_TITLE,
                STUDY_PLAN_EDITING_NOT_EXIST_BODY, Constants.GenericStrings.OKAY);
            return;
        }

        editingStudyPlan.Status = StudyPlanStatus;
        editingStudyPlan.Academic = AcademicTerm;
        editingStudyPlan.Seasonal = SeasonalTerm;
        editingStudyPlan.Year = Year;
        editingStudyPlan.DueDate = DueDate;
        editingStudyPlan.Notes = Notes;
        editingStudyPlan.UpdatedAt = DateTime.Now;

        await SaveChanges(editingStudyPlan);
    }
    #endregion
}
