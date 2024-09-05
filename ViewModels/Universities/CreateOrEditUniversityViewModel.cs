using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;
using Microsoft.EntityFrameworkCore;

namespace CourseEquivalencyDesktop.ViewModels.Universities;

public partial class CreateOrEditUniversityViewModel : BaseCreateOrEditViewModel<University>
{
    #region Constants
    private const string CREATE_TEXT = "Create University";
    private const string EDIT_TEXT = "Edit University";
    private const string UNIVERSITY_NAME_EXISTS_TITLE = "University Exists";
    private const string UNIVERSITY_NAME_EXISTS_BODY = "A university with this name already exists.";
    private const string UNIVERSITY_EDITING_NOT_EXIST_TITLE = "University Doesn't Exist";
    private const string UNIVERSITY_EDITING_NOT_EXIST_BODY = "The university you are trying to edit does not exist.";
    #endregion

    #region Properties
    protected override string FailedSaveTitle => "University Changes Failed";
    protected override string FailedSaveBody => "An error occurred and the university changes could not be made.";

    #region Observable Properties
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateOrEditCommand))]
    [Required(AllowEmptyStrings = false)]
    private string name = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateOrEditCommand))]
    [Url]
    private string? url;
    #endregion
    #endregion

    #region Constructors
    public CreateOrEditUniversityViewModel()
    {
        WindowAndButtonText = CREATE_TEXT;
    }

    public CreateOrEditUniversityViewModel(University? university, DatabaseService databaseService,
        GenericDialogService genericDialogService) : base(university, databaseService, genericDialogService)
    {
        WindowAndButtonText = IsCreate ? CREATE_TEXT : EDIT_TEXT;

        Name = university?.Name ?? string.Empty;
        Url = university?.Url;

        // Ensure the button is disabled if invalid but don't trigger errors as they haven't performed any actions yet
        CreateOrEditCommand.NotifyCanExecuteChanged();
    }
    #endregion

    #region Handlers
    partial void OnNameChanged(string value)
    {
        ValidateProperty(value, nameof(Name));
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
        return base.CanCreateOrEdit() && !string.IsNullOrWhiteSpace(Name);
    }
    #endregion

    #region Commands
    protected override async Task CreateOrEditInherited()
    {
        var preparedName = Name.Trim();
        var preparedUrl = Url?.Trim();
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
            var doesNameExist = await DatabaseService.Universities.AnyAsync(uni => uni.Name == preparedName);
            if (doesNameExist)
            {
                await GenericDialogService.OpenGenericDialog(UNIVERSITY_NAME_EXISTS_TITLE,
                    UNIVERSITY_NAME_EXISTS_BODY, Constants.GenericStrings.OKAY);
                return;
            }

            var entityEntry = await DatabaseService.AddAsync(new University
            {
                Name = preparedName,
                Url = preparedUrl
            });
            await SaveChanges(entityEntry.Entity);
        }

        async Task Update()
        {
            var editingUniversity =
                await DatabaseService.Universities.FindAsync(Item!.Id);
            if (editingUniversity is null)
            {
                await GenericDialogService.OpenGenericDialog(UNIVERSITY_EDITING_NOT_EXIST_TITLE,
                    UNIVERSITY_EDITING_NOT_EXIST_BODY, Constants.GenericStrings.OKAY);
                return;
            }

            var doesNameExist =
                await DatabaseService.Universities.AnyAsync(uni =>
                    uni.Name == preparedName && uni.Id != editingUniversity.Id);
            if (doesNameExist)
            {
                await GenericDialogService.OpenGenericDialog(UNIVERSITY_NAME_EXISTS_TITLE,
                    UNIVERSITY_NAME_EXISTS_BODY, Constants.GenericStrings.OKAY);
                return;
            }

            editingUniversity.Name = preparedName;
            editingUniversity.Url = preparedUrl;
            await SaveChanges(editingUniversity);
        }
    }
    #endregion
}
