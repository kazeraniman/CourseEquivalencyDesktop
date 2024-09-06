using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;
using MiniSoftware;

namespace CourseEquivalencyDesktop.ViewModels.StudyPlans;

public partial class EditStudyPlanViewModel : BaseCreateOrEditViewModel<StudyPlan>
{
    #region Constants
    private const string CREDIT_TRANSFER_MEMO_TEMPLATE_PATH = "DocumentTemplates/CreditTransferMemoTemplate.docx";

    private const string EDIT_TEXT = "Edit Study Plan";
    private const string STUDY_PLAN_EDITING_NOT_EXIST_TITLE = "Study Plan Doesn't Exist";
    private const string STUDY_PLAN_EDITING_NOT_EXIST_BODY = "The study plan you are trying to edit does not exist.";
    private const string STUDY_PLAN_COMPLETE_MISSING_EQUIVALENCIES_TITLE = "Study Plan Can't Be Complete";

    private const string STUDY_PLAN_COMPLETE_MISSING_EQUIVALENCIES_BODY =
        "They study plan can't be marked as complete as there are still home university courses without and equivalent in the destination university.";

    #region Credit Transfer Memo
    private const string CREDIT_TRANSFER_MEMO_EXPORTED_TITLE = "Credit Transfer Memo Exported";

    private const string CREDIT_TRANSFER_MEMO_EXPORTED_BODY =
        "The credit transfer memo has been successfully exported.";

    private const string CREDIT_TRANSFER_MEMO_EXPORT_TITLE = "Export Credit Transfer Memo";
    private const string CREDIT_TRANSFER_MEMO_DATE_TEMPLATE_TAG = "Date";
    private const string CREDIT_TRANSFER_MEMO_STUDENT_NAME_TEMPLATE_TAG = "Student_Name";
    private const string CREDIT_TRANSFER_MEMO_STUDENT_ID_TEMPLATE_TAG = "Student_ID";
    private const string CREDIT_TRANSFER_MEMO_PLAN_TEMPLATE_TAG = "Plan";
    private const string CREDIT_TRANSFER_MEMO_HOME_UNIVERSITY_TEMPLATE_TAG = "Home_University";
    private const string CREDIT_TRANSFER_MEMO_DESTINATION_UNIVERSITY_TEMPLATE_TAG = "Destination_University";
    private const string CREDIT_TRANSFER_MEMO_TERM_TEMPLATE_TAG = "Term";
    private const string CREDIT_TRANSFER_MEMO_ADVISOR_TEMPLATE_TAG = "Advisor";
    private const string CREDIT_TRANSFER_MEMO_COURSES_TEMPLATE_TAG = "Courses";
    private const string CREDIT_TRANSFER_MEMO_DEFAULT_ADVISOR_NAME = "SET NAME IN SETTINGS";
    private const string CREDIT_TRANSFER_MEMO_DEFAULT_ADVISOR_DEPARTMENT = "SET DEPARTMENT IN SETTINGS";
    #endregion
    #endregion

    #region Fields
    public readonly Interaction<bool?, bool?> RequestedCourseEquivalencyInteraction = new();
    public readonly Interaction<bool?, bool?> AddedHomeCourseInteraction = new();
    public readonly Interaction<bool?, bool?> AddedDestinationCourseInteraction = new();

    private readonly ObservableCollection<Course> homeUniversityCourseOptions = [];
    private readonly ObservableCollection<Course> destinationUniversityCourseOptions = [];

    private Course? equivalentCourse;

    private readonly EquivalentCourseComparer equivalentCourseComparer;

    private readonly FileDialogService fileDialogService;
    private readonly UserSettingsService userSettingsService;
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

        fileDialogService = new FileDialogService();
        userSettingsService = new UserSettingsService();

        equivalentCourseComparer = new EquivalentCourseComparer(new List<Course>());

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

    public EditStudyPlanViewModel(StudyPlan? studyPlan, FileDialogService fileDialogService,
        UserSettingsService userSettingsService, DatabaseService databaseService,
        GenericDialogService genericDialogService) : base(studyPlan, databaseService, genericDialogService)
    {
        if (studyPlan is null)
        {
            throw new UnreachableException("Study plan which is null should not be editable.");
        }

        this.userSettingsService = userSettingsService;
        this.fileDialogService = fileDialogService;

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

        equivalentCourseComparer = new EquivalentCourseComparer(HomeUniversityCourses);
        SortDestinationCoursesByEquivalency();

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

        if (StudyPlanStatus == StudyPlanStatus.Complete)
        {
            var destinationUniversityEquivalencies = new HashSet<int>(DestinationUniversityCourses
                .SelectMany(destinationCourse => destinationCourse.Equivalencies
                    .Select(destinationCourseEquivalency => destinationCourseEquivalency.Id)));

            if (HomeUniversityCourses.Any(homeUniversityCourse =>
                    !destinationUniversityEquivalencies.Contains(homeUniversityCourse.Id)))
            {
                await GenericDialogService.OpenGenericDialog(STUDY_PLAN_COMPLETE_MISSING_EQUIVALENCIES_TITLE,
                    STUDY_PLAN_COMPLETE_MISSING_EQUIVALENCIES_BODY, Constants.GenericStrings.OKAY);
                return;
            }
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
        if (SelectedHomeUniversityCourse is not null && !HomeUniversityCourses.Contains(SelectedHomeUniversityCourse))
        {
            HomeUniversityCourses.Add(SelectedHomeUniversityCourse);
        }

        AddedHomeCourseInteraction.HandleAsync(null);
    }

    [RelayCommand]
    private void AddDestinationCourse()
    {
        if (SelectedDestinationUniversityCourse is not null &&
            !DestinationUniversityCourses.Contains(SelectedDestinationUniversityCourse))
        {
            DestinationUniversityCourses.Add(SelectedDestinationUniversityCourse);
            SortDestinationCoursesByEquivalency();
        }

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
        SortDestinationCoursesByEquivalency();
    }

    [RelayCommand]
    private void FindEquivalentCourse(Course course)
    {
        equivalentCourse = course;
        DestinationUniversityCourseOptionsView.Refresh();
        RequestedCourseEquivalencyInteraction.HandleAsync(null);
    }

    [RelayCommand]
    private async Task ExportCreditTransferMemo()
    {
        var exportFile = await fileDialogService.SaveFileDialog(CREDIT_TRANSFER_MEMO_EXPORT_TITLE,
            $"CreditTransferMemo-{Student.StudentId}-{Student.Name}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}",
            FileDialogService.WORD_DOCUMENT_DEFAULT_EXTENSION, FileDialogService.WordDocumentFilePickerFileType);

        if (exportFile is null)
        {
            return;
        }

        var maxCourses = Math.Max(HomeUniversityCourses.Count, DestinationUniversityCourses.Count);
        var coursesTemplate = new List<Dictionary<string, object>>();
        for (var i = 0; i < maxCourses; i++)
        {
            coursesTemplate.Add(new Dictionary<string, object>
            {
                {
                    CREDIT_TRANSFER_MEMO_DESTINATION_UNIVERSITY_TEMPLATE_TAG,
                    i < DestinationUniversityCourses.Count
                        ? $"{DestinationUniversityCourses[i].CourseId} {DestinationUniversityCourses[i].Name}"
                        : string.Empty
                },
                {
                    CREDIT_TRANSFER_MEMO_HOME_UNIVERSITY_TEMPLATE_TAG,
                    i < HomeUniversityCourses.Count
                        ? $"{HomeUniversityCourses[i].CourseId} {HomeUniversityCourses[i].Name}"
                        : string.Empty
                }
            });
        }

        var templateValues = new Dictionary<string, object>
        {
            { CREDIT_TRANSFER_MEMO_DATE_TEMPLATE_TAG, DateTime.Today.ToString("MMMM d, yyyy") },
            { CREDIT_TRANSFER_MEMO_STUDENT_NAME_TEMPLATE_TAG, Student.Name },
            { CREDIT_TRANSFER_MEMO_STUDENT_ID_TEMPLATE_TAG, Student.StudentId },
            {
                CREDIT_TRANSFER_MEMO_PLAN_TEMPLATE_TAG,
                $"{Student.Program.GetProgramTypeString()}/{Student.Stream.GetStreamTypeString()}"
            },
            { CREDIT_TRANSFER_MEMO_DESTINATION_UNIVERSITY_TEMPLATE_TAG, DestinationUniversity.Name },
            { CREDIT_TRANSFER_MEMO_TERM_TEMPLATE_TAG, AcademicTerm.GetAcademicTermString() },
            {
                CREDIT_TRANSFER_MEMO_ADVISOR_TEMPLATE_TAG,
                $"{userSettingsService.UserFullName ?? CREDIT_TRANSFER_MEMO_DEFAULT_ADVISOR_NAME} ({userSettingsService.UserDepartment ?? CREDIT_TRANSFER_MEMO_DEFAULT_ADVISOR_DEPARTMENT})"
            },
            { CREDIT_TRANSFER_MEMO_COURSES_TEMPLATE_TAG, coursesTemplate }
        };

        MiniWord.SaveAsByTemplate(exportFile.Path.LocalPath,
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CREDIT_TRANSFER_MEMO_TEMPLATE_PATH), templateValues);

        await GenericDialogService.OpenGenericDialog(CREDIT_TRANSFER_MEMO_EXPORTED_TITLE,
            CREDIT_TRANSFER_MEMO_EXPORTED_BODY, Constants.GenericStrings.OKAY);
    }
    #endregion

    #region Helpers
    private void SortDestinationCoursesByEquivalency()
    {
        var newDestinationUniversityCourses = DestinationUniversityCourses
            .OrderBy(course => course, equivalentCourseComparer)
            .ToList();
        DestinationUniversityCourses.Clear();
        DestinationUniversityCourses.AddRange(newDestinationUniversityCourses);
    }
    #endregion
}
