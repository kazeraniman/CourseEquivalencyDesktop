using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;
using Microsoft.EntityFrameworkCore;

namespace CourseEquivalencyDesktop.ViewModels.Students;

public partial class CreateOrEditStudentViewModel : BaseCreateOrEditViewModel<Student>
{
    #region Constants
    private const string CREATE_TEXT = "Create Student";
    private const string EDIT_TEXT = "Edit Student";
    private const string STUDENT_ID_EXISTS_TITLE = "Student Exists";
    private const string STUDENT_ID_EXISTS_BODY = "A student with this student ID already exists in this university.";
    private const string STUDENT_EDITING_NOT_EXIST_TITLE = "Student Doesn't Exist";
    private const string STUDENT_EDITING_NOT_EXIST_BODY = "The student you are trying to edit does not exist.";
    #endregion

    #region Properties
    protected override string FailedSaveTitle => "Student Changes Failed";
    protected override string FailedSaveBody => "An error occurred and the student changes could not be made.";

    #region Observable Properties
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateOrEditCommand))]
    [Required(AllowEmptyStrings = false)]
    private string studentId = string.Empty;

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
    [Required]
    private Student.ProgramType program;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateOrEditCommand))]
    [Required]
    private Student.StreamType stream;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateOrEditCommand))]
    [EmailAddress]
    private string? email;

    [ObservableProperty]
    private string? notes;

    [ObservableProperty]
    private University[] universities;

    [ObservableProperty]
    private Student.ProgramType[] programTypes = Enum.GetValues<Student.ProgramType>();

    [ObservableProperty]
    private Student.StreamType[] streamTypes = Enum.GetValues<Student.StreamType>();
    #endregion
    #endregion

    #region Constructors
    public CreateOrEditStudentViewModel()
    {
        WindowAndButtonText = CREATE_TEXT;
        Universities = [];
        University = new University();
    }

    public CreateOrEditStudentViewModel(Student? student, DatabaseService databaseService,
        GenericDialogService genericDialogService) : base(student, databaseService, genericDialogService)
    {
        WindowAndButtonText = IsCreate ? CREATE_TEXT : EDIT_TEXT;

        Name = student?.Name ?? string.Empty;
        StudentId = student?.StudentId ?? string.Empty;
        Email = student?.Email;
        Notes = student?.Notes;
        Universities = databaseService.Universities
            .OrderBy(uni => uni.Name)
            .ToArray();
        University = student?.University ?? Universities[0];
        Program = student?.Program ?? ProgramTypes[0];
        Stream = student?.Stream ?? StreamTypes[0];

        // Ensure the button is disabled if invalid but don't trigger errors as they haven't performed any actions yet
        CreateOrEditCommand.NotifyCanExecuteChanged();
    }
    #endregion

    #region Handlers
    partial void OnNameChanged(string value)
    {
        ValidateProperty(value, nameof(Name));
    }

    partial void OnStudentIdChanged(string value)
    {
        ValidateProperty(value, nameof(StudentId));
    }

    partial void OnEmailChanged(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            ClearErrors(nameof(Email));
            return;
        }

        ValidateProperty(value, nameof(Email));
    }
    #endregion

    #region Command Execution Checks
    protected override bool CanCreateOrEdit()
    {
        return base.CanCreateOrEdit() && !(string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(StudentId));
    }
    #endregion

    #region Commands
    protected override async Task CreateOrEditInherited()
    {
        var preparedName = Name.Trim();
        var preparedEmail = Email?.Trim();
        var preparedStudentId = StudentId.Trim();
        var preparedNotes = Notes?.Trim();
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
            var doesStudentIdExist = await DatabaseService.Students.AnyAsync(stu =>
                stu.UniversityId == University.Id && stu.StudentId == preparedStudentId);
            if (doesStudentIdExist)
            {
                await GenericDialogService.OpenGenericDialog(STUDENT_ID_EXISTS_TITLE,
                    STUDENT_ID_EXISTS_BODY, Constants.GenericStrings.OKAY);
                return;
            }

            var entityEntry = await DatabaseService.AddAsync(new Student
            {
                University = University,
                Name = preparedName,
                StudentId = preparedStudentId,
                Program = Program,
                Stream = Stream,
                Email = preparedEmail,
                Notes = preparedNotes
            });
            await SaveChanges(entityEntry.Entity);
        }

        async Task Update()
        {
            var editingStudent =
                await DatabaseService.Students.FirstOrDefaultAsync(stu => stu.Id == Item!.Id);
            if (editingStudent is null)
            {
                await GenericDialogService.OpenGenericDialog(STUDENT_EDITING_NOT_EXIST_TITLE,
                    STUDENT_EDITING_NOT_EXIST_BODY, Constants.GenericStrings.OKAY);
                return;
            }

            var doesStudentIdExist = await DatabaseService.Students.AnyAsync(stu =>
                stu.UniversityId == University.Id && stu.StudentId == preparedStudentId && stu.Id != editingStudent.Id);
            if (doesStudentIdExist)
            {
                await GenericDialogService.OpenGenericDialog(STUDENT_ID_EXISTS_TITLE,
                    STUDENT_ID_EXISTS_BODY, Constants.GenericStrings.OKAY);
                return;
            }

            editingStudent.University = University;
            editingStudent.Name = preparedName;
            editingStudent.StudentId = preparedStudentId;
            editingStudent.Program = Program;
            editingStudent.Stream = Stream;
            editingStudent.Email = preparedEmail;
            editingStudent.Notes = preparedNotes;
            await SaveChanges(editingStudent);
        }
    }
    #endregion
}
