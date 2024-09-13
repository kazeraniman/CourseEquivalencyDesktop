using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;
using Microsoft.EntityFrameworkCore;
using MiniSoftware;

namespace CourseEquivalencyDesktop.ViewModels.StudyPlans;

public partial class EditStudyPlanViewModel : BaseCreateOrEditViewModel<StudyPlan>
{
    #region Constants
    private const string EDIT_TEXT = "Edit Study Plan";
    private const string STUDY_PLAN_EDITING_NOT_EXIST_TITLE = "Study Plan Doesn't Exist";
    private const string STUDY_PLAN_EDITING_NOT_EXIST_BODY = "The study plan you are trying to edit does not exist.";
    private const string STUDY_PLAN_COMPLETE_MISSING_EQUIVALENCIES_TITLE = "Study Plan Can't Be Complete";

    private const string STUDY_PLAN_COMPLETE_MISSING_EQUIVALENCIES_BODY =
        "They study plan can't be marked as complete as there are still home university courses without and equivalent in the destination university.";

    #region Export
    private const string STUDY_PLAN_EXPORT_NO_EMAIL_TITLE = "Student Email Missing";
    private const string STUDY_PLAN_EXPORT_NO_EMAIL_BODY = "The student's email must be set to export.";
    private const string STUDY_PLAN_EXPORT_NO_DATES_TITLE = "Exchange Dates Missing";
    private const string STUDY_PLAN_EXPORT_NO_DATES_BODY = "The exchange start and end dates must be set to export.";

    private const string CREDIT_TRANSFER_MEMO_EXPORTED_TITLE = "Credit Transfer Memo Exported";

    private const string CREDIT_TRANSFER_MEMO_EXPORTED_BODY =
        "The credit transfer memo has been successfully exported.";

    private const string PROPOSED_STUDY_PLAN_EXPORTED_TITLE = "Proposed Study Plan Exported";

    private const string PROPOSED_STUDY_PLAN_EXPORTED_BODY =
        "The proposed study plan has been successfully exported.";

    private const string STUDY_PLAN_EXPORT_SETTINGS_MISSING_TITLE = "Export Settings Missing";

    private const string STUDY_PLAN_EXPORT_SETTINGS_MISSING_BODY =
        "Some settings required for filling out the template are missing. Please set them in the settings page.";

    private const string STUDY_PLAN_EXPORT_TEMPLATE_FILE_MISSING_TITLE = "Template Missing";

    private const string STUDY_PLAN_EXPORT_TEMPLATE_FILE_MISSING_BODY =
        "The template selected is set in the settings but the file could not be found. Please double-check the location.";

    private const string CREDIT_TRANSFER_MEMO = "CreditTransferMemo";
    private const string CREDIT_TRANSFER_MEMO_EXPORT_TITLE = "Export Credit Transfer Memo";
    private const string PROPOSED_STUDY_PLAN = "ProposedStudyPlan";
    private const string PROPOSED_STUDY_PLAN_EXPORT_TITLE = "Export Proposed Study Plan";
    private const string DATE_TEMPLATE_TAG = "Date";
    private const string STUDENT_NAME_TEMPLATE_TAG = "Student_Name";
    private const string STUDENT_ID_TEMPLATE_TAG = "Student_ID";
    private const string STUDENT_EMAIL_TEMPLATE_TAG = "Student_Email";
    private const string PROGRAM_TEMPLATE_TAG = "Program";
    private const string STREAM_TEMPLATE_TAG = "Stream";
    private const string HOME_UNIVERSITY_TEMPLATE_TAG = "Home_University";
    private const string DESTINATION_UNIVERSITY_TEMPLATE_TAG = "Destination_University";
    private const string TERM_TEMPLATE_TAG = "Term";
    private const string ADVISOR_NAME_TEMPLATE_TAG = "Advisor_Name";
    private const string ADVISOR_DEPARTMENT_TEMPLATE_TAG = "Advisor_Department";
    private const string COURSES_TEMPLATE_TAG = "Courses";
    private const string GRADUATION_YEAR_TEMPLATE_TAG = "Graduation_Year";
    private const string DESTINATION_COUNTRY_TEMPLATE_TAG = "Destination_Country";
    private const string START_DATE_TEMPLATE_TAG = "Start_Date";
    private const string END_DATE_TEMPLATE_TAG = "End_Date";
    private const string LAST_TERM_TEMPLATE_TAG = "Last_Term";
    private const string DATE_FORMAT_STRING = "dd-MMM-yyyy";
    private const string MISSING_DEFAULT_STRING = "MISSING";
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
    private DateTime? exchangeStartDate;

    [ObservableProperty]
    private DateTime? exchangeEndDate;

    [ObservableProperty]
    [Required]
    private StudyPlan.AcademicTerm lastCompletedAcademicTerm;

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
        UserSettingsService userSettingsService, DatabaseService databaseService) : base(studyPlan, databaseService)
    {
        if (studyPlan is null)
        {
            throw new UnreachableException("Study plan which is null should not be editable.");
        }

        this.userSettingsService = userSettingsService;
        this.fileDialogService = fileDialogService;

        WindowAndButtonText = EDIT_TEXT;

        // TODO: Use async loads and have a loading component displayed while getting ready
        Student = databaseService.Students
                      .Include(stu => stu.University)
                      .ThenInclude(uni => uni.Courses)
                      .ThenInclude(course => course.Equivalencies)
                      .SingleOrDefault(stu => stu.Id == studyPlan.Student.Id)
                  ?? studyPlan.Student;
        DestinationUniversity = databaseService.Universities
                                    .Include(uni => uni.Courses)
                                    .ThenInclude(course => course.Equivalencies)
                                    .SingleOrDefault(uni => uni.Id == studyPlan.DestinationUniversity.Id)
                                ?? studyPlan.DestinationUniversity;
        StudyPlanStatus = studyPlan.Status;
        AcademicTerm = studyPlan.Academic;
        SeasonalTerm = studyPlan.Seasonal;
        Year = studyPlan.Year;
        DueDate = studyPlan.DueDate;
        Notes = studyPlan.Notes;
        ExchangeStartDate = studyPlan.ExchangeStartDate;
        ExchangeEndDate = studyPlan.ExchangeEndDate;
        LastCompletedAcademicTerm = studyPlan.LastCompletedAcademicTerm;

        var freshStudyPlan = databaseService.StudyPlans
                                 .Include(sp => sp.HomeUniversityCourses)
                                 .Include(sp => sp.DestinationUniversityCourses)
                                 .SingleOrDefault(sp => sp.Id == studyPlan.Id)
                             ?? studyPlan;
        HomeUniversityCourses.AddRange(freshStudyPlan.HomeUniversityCourses);
        DestinationUniversityCourses.AddRange(freshStudyPlan.DestinationUniversityCourses);

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
            ShowNotification(STUDY_PLAN_EDITING_NOT_EXIST_TITLE, STUDY_PLAN_EDITING_NOT_EXIST_BODY,
                NotificationType.Error);
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
                ShowNotification(STUDY_PLAN_COMPLETE_MISSING_EQUIVALENCIES_TITLE,
                    STUDY_PLAN_COMPLETE_MISSING_EQUIVALENCIES_BODY, NotificationType.Error);
                return;
            }
        }

        editingStudyPlan.Status = StudyPlanStatus;
        editingStudyPlan.Academic = AcademicTerm;
        editingStudyPlan.Seasonal = SeasonalTerm;
        editingStudyPlan.Year = Year;
        editingStudyPlan.DueDate = DueDate;
        editingStudyPlan.ExchangeStartDate = ExchangeStartDate;
        editingStudyPlan.ExchangeEndDate = ExchangeEndDate;
        editingStudyPlan.LastCompletedAcademicTerm = LastCompletedAcademicTerm;
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
        await Export(CREDIT_TRANSFER_MEMO, CREDIT_TRANSFER_MEMO_EXPORT_TITLE,
            userSettingsService.CreditTransferMemoTemplateFilePath, CREDIT_TRANSFER_MEMO_EXPORTED_TITLE,
            CREDIT_TRANSFER_MEMO_EXPORTED_BODY);
    }

    [RelayCommand]
    private async Task ExportProposedStudyPlan()
    {
        await Export(PROPOSED_STUDY_PLAN, PROPOSED_STUDY_PLAN_EXPORT_TITLE,
            userSettingsService.ProposedStudyPlanTemplateFilePath, PROPOSED_STUDY_PLAN_EXPORTED_TITLE,
            PROPOSED_STUDY_PLAN_EXPORTED_BODY);
    }
    #endregion

    #region Helpers
    private async Task Export(string exportType, string saveFileDialogTitle, string? templatePath,
        string exportedSuccessTitle, string exportedSuccessBody)
    {
        if (string.IsNullOrEmpty(templatePath) || string.IsNullOrWhiteSpace(userSettingsService.UserFullName) ||
            string.IsNullOrWhiteSpace(userSettingsService.UserDepartment))
        {
            ShowNotification(STUDY_PLAN_EXPORT_SETTINGS_MISSING_TITLE, STUDY_PLAN_EXPORT_SETTINGS_MISSING_BODY,
                NotificationType.Error);
            return;
        }

        var templateFile = new Uri(templatePath).LocalPath;
        if (!File.Exists(templateFile))
        {
            ShowNotification(STUDY_PLAN_EXPORT_TEMPLATE_FILE_MISSING_TITLE,
                STUDY_PLAN_EXPORT_TEMPLATE_FILE_MISSING_BODY, NotificationType.Error);
            return;
        }

        if (exportType == PROPOSED_STUDY_PLAN)
        {
            if (string.IsNullOrWhiteSpace(Student.Email))
            {
                ShowNotification(STUDY_PLAN_EXPORT_NO_EMAIL_TITLE, STUDY_PLAN_EXPORT_NO_EMAIL_BODY,
                    NotificationType.Error);
                return;
            }

            if (ExchangeStartDate is null || ExchangeEndDate is null)
            {
                ShowNotification(STUDY_PLAN_EXPORT_NO_DATES_TITLE, STUDY_PLAN_EXPORT_NO_DATES_BODY,
                    NotificationType.Error);
                return;
            }
        }

        var exportFile = await fileDialogService.SaveFileDialog(saveFileDialogTitle,
            $"{exportType}-{Student.StudentId}-{Student.Name}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}",
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
                    DESTINATION_UNIVERSITY_TEMPLATE_TAG,
                    i < DestinationUniversityCourses.Count
                        ? $"{DestinationUniversityCourses[i].CourseId} {DestinationUniversityCourses[i].Name}"
                        : string.Empty
                },
                {
                    HOME_UNIVERSITY_TEMPLATE_TAG,
                    i < HomeUniversityCourses.Count
                        ? $"{HomeUniversityCourses[i].CourseId} {HomeUniversityCourses[i].Name}"
                        : string.Empty
                }
            });
        }

        var templateValues = new Dictionary<string, object>
        {
            { DATE_TEMPLATE_TAG, FormatDate(DateTime.Today) },
            { STUDENT_NAME_TEMPLATE_TAG, Student.Name },
            { STUDENT_ID_TEMPLATE_TAG, Student.StudentId },
            { STUDENT_EMAIL_TEMPLATE_TAG, Student.Email ?? MISSING_DEFAULT_STRING },
            { PROGRAM_TEMPLATE_TAG, Student.Program.GetProgramTypeString() },
            { STREAM_TEMPLATE_TAG, Student.Stream.GetStreamTypeString() },
            { HOME_UNIVERSITY_TEMPLATE_TAG, Student.University.Name },
            { DESTINATION_UNIVERSITY_TEMPLATE_TAG, DestinationUniversity.Name },
            { TERM_TEMPLATE_TAG, AcademicTerm.GetAcademicTermString() },
            { ADVISOR_NAME_TEMPLATE_TAG, userSettingsService.UserFullName },
            { ADVISOR_DEPARTMENT_TEMPLATE_TAG, userSettingsService.UserDepartment },
            { GRADUATION_YEAR_TEMPLATE_TAG, Student.ExpectedGraduationYear },
            { DESTINATION_COUNTRY_TEMPLATE_TAG, DestinationUniversity.Country },
            { START_DATE_TEMPLATE_TAG, FormatDate(ExchangeStartDate) },
            { END_DATE_TEMPLATE_TAG, FormatDate(ExchangeEndDate) },
            { LAST_TERM_TEMPLATE_TAG, LastCompletedAcademicTerm.GetAcademicTermString() },
            { COURSES_TEMPLATE_TAG, coursesTemplate }
        };

        MiniWord.SaveAsByTemplate(exportFile.Path.LocalPath, templateFile, templateValues);

        ShowNotification(exportedSuccessTitle, exportedSuccessBody, NotificationType.Success);

        Process.Start("explorer.exe", $"/select, \"{exportFile.Path.LocalPath}\"");
    }

    private void SortDestinationCoursesByEquivalency()
    {
        var newDestinationUniversityCourses = DestinationUniversityCourses
            .OrderBy(course => course, equivalentCourseComparer)
            .ToList();
        DestinationUniversityCourses.Clear();
        DestinationUniversityCourses.AddRange(newDestinationUniversityCourses);
    }

    private string FormatDate(DateTime? date)
    {
        return date?.ToString(DATE_FORMAT_STRING).Replace(".", "").ToUpper() ?? MISSING_DEFAULT_STRING;
    }
    #endregion
}
