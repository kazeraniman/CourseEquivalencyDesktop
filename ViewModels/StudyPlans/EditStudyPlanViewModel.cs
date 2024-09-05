using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    #region Fields
    public readonly Interaction<bool?, bool?> RequestedCourseEquivalencyInteraction = new();
    public readonly Interaction<bool?, bool?> AddedHomeCourseInteraction = new();
    public readonly Interaction<bool?, bool?> AddedDestinationCourseInteraction = new();

    private readonly ObservableCollection<Course> homeUniversityCourseOptions = [];
    private readonly ObservableCollection<Course> destinationUniversityCourseOptions = [];

    private Course? equivalentCourse;
    #endregion

    #region Properties
    protected override string FailedSaveTitle => "Study Plan Changes Failed";
    protected override string FailedSaveBody => "An error occurred and the study plan changes could not be made.";

    public DataGridCollectionView HomeUniversityCourseOptionsView { get; init; }
    public DataGridCollectionView DestinationUniversityCourseOptionsView { get; init; }

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

    [ObservableProperty]
    private string? notes;

    [ObservableProperty]
    private ObservableCollection<Course> homeUniversityCourses = [];

    [ObservableProperty]
    private ObservableCollection<Course> destinationUniversityCourses = [];

    [ObservableProperty]
    private StudyPlan.AcademicTerm[] academicTerms = Enum.GetValues<StudyPlan.AcademicTerm>();

    [ObservableProperty]
    private StudyPlan.SeasonalTerm[] seasonalTerms = Enum.GetValues<StudyPlan.SeasonalTerm>();

    [ObservableProperty]
    private Course? selectedHomeUniversityCourse;

    [ObservableProperty]
    private Course? selectedDestinationUniversityCourse;
    #endregion
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
        HomeUniversityCourseOptionsView = new DataGridCollectionView(homeUniversityCourseOptions);
        DestinationUniversityCourseOptionsView = new DataGridCollectionView(destinationUniversityCourseOptions);
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

        HomeUniversityCourses.AddRange(studyPlan.HomeUniversityCourses);
        DestinationUniversityCourses.AddRange(studyPlan.DestinationUniversityCourses);

        homeUniversityCourseOptions.AddRange(Student.University.Courses
            .OrderBy(course => course.Name)
            .ThenBy(course => course.CourseId));
        destinationUniversityCourseOptions.AddRange(DestinationUniversity.Courses
            .OrderBy(course => course.Name)
            .ThenBy(course => course.CourseId));

        HomeUniversityCourseOptionsView = new DataGridCollectionView(homeUniversityCourseOptions);
        DestinationUniversityCourseOptionsView = new DataGridCollectionView(destinationUniversityCourseOptions)
        {
            Filter = DestinationUniversityCourseOptionFilter
        };

        // Ensure the button is disabled if invalid but don't trigger errors as they haven't performed any actions yet
        CreateOrEditCommand.NotifyCanExecuteChanged();
    }
    #endregion

    #region Filters
    private bool DestinationUniversityCourseOptionFilter(object arg)
    {
        if (arg is not Course course)
        {
            return false;
        }

        return equivalentCourse is not null &&
               course.Equivalencies.Contains(equivalentCourse);
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

        editingStudyPlan.HomeUniversityCourses.Clear();
        foreach (var homeUniversityCourse in HomeUniversityCourses)
        {
            editingStudyPlan.HomeUniversityCourses.Add(homeUniversityCourse);
        }

        editingStudyPlan.DestinationUniversityCourses.Clear();
        foreach (var destinationUniversityCourse in DestinationUniversityCourses)
        {
            editingStudyPlan.DestinationUniversityCourses.Add(destinationUniversityCourse);
        }

        await SaveChanges(editingStudyPlan);
    }

    [RelayCommand]
    private void AddHomeCourse()
    {
        if (SelectedHomeUniversityCourse is null || HomeUniversityCourses.Contains(SelectedHomeUniversityCourse))
        {
            return;
        }

        HomeUniversityCourses.Add(SelectedHomeUniversityCourse);
        AddedHomeCourseInteraction.HandleAsync(null);
    }

    [RelayCommand]
    private void AddDestinationCourse()
    {
        if (SelectedDestinationUniversityCourse is null ||
            DestinationUniversityCourses.Contains(SelectedDestinationUniversityCourse))
        {
            return;
        }

        DestinationUniversityCourses.Add(SelectedDestinationUniversityCourse);
        AddedDestinationCourseInteraction.HandleAsync(null);
    }

    [RelayCommand]
    private void RemoveHomeCourse(Course course)
    {
        HomeUniversityCourses.Remove(course);
    }

    [RelayCommand]
    private void RemoveDestinationCourse(Course course)
    {
        DestinationUniversityCourses.Remove(course);
    }

    [RelayCommand]
    private void FindEquivalentCourse(Course course)
    {
        equivalentCourse = course;
        DestinationUniversityCourseOptionsView.Refresh();
        RequestedCourseEquivalencyInteraction.HandleAsync(null);
    }
    #endregion
}
