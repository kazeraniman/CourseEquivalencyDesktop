using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;
using Microsoft.EntityFrameworkCore;

namespace CourseEquivalencyDesktop.ViewModels.Universities;

public partial class CreateOrEditUniversityViewModel : ViewModelBase
{
    public class CreateOrEditUniversityEventArgs : EventArgs
    {
        public University? University { get; init; }
    }

    #region Constants
    private const string CREATE_TEXT = "Create University";
    private const string EDIT_TEXT = "Edit University";
    private const string UNIVERSITY_NAME_EXISTS_TITLE = "University Exists";
    private const string UNIVERSITY_NAME_EXISTS_BODY = "A university with this name already exists.";
    private const string UNIVERSITY_EDITING_NOT_EXIST_TITLE = "University Doesn't Exist";
    private const string UNIVERSITY_EDITING_NOT_EXIST_BODY = "The university you are trying to edit does not exist.";
    private const string UNIVERSITY_FAILED_SAVE_TITLE = "University Changes Failed";

    private const string UNIVERSITY_FAILED_SAVE_BODY =
        "An error occurred and the university changes could not be made.";
    #endregion

    #region Fields
    private readonly DatabaseService databaseService;
    private readonly GenericDialogService genericDialogService;

    private readonly University? university;
    private readonly bool isCreate;
    #endregion

    #region Properties
    #region Observable Properties
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateOrEditCommand))]
    [Required]
    private string name = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateOrEditCommand))]
    [Url]
    private string? url;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    [NotifyCanExecuteChangedFor(nameof(CreateOrEditCommand))]
    private bool isCreating;

    [ObservableProperty]
    private string windowAndButtonText;
    #endregion
    #endregion

    #region Events
    public event EventHandler? OnRequestCloseWindow;
    #endregion

    #region Constructors
    public CreateOrEditUniversityViewModel()
    {
        Utility.Utility.AssertDesignMode();

        databaseService = new DatabaseService();
        genericDialogService = new GenericDialogService();
        WindowAndButtonText = CREATE_TEXT;
    }

    public CreateOrEditUniversityViewModel(University? university, DatabaseService databaseService,
        GenericDialogService genericDialogService)
    {
        this.databaseService = databaseService;
        this.genericDialogService = genericDialogService;
        this.university = university;
        isCreate = university is null;
        WindowAndButtonText = isCreate ? CREATE_TEXT : EDIT_TEXT;

        Name = university?.Name ?? string.Empty;
        Url = university?.Url;
    }
    #endregion

    #region Handlers
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
        return !(IsCreating || HasErrors || string.IsNullOrWhiteSpace(Name));
    }
    #endregion

    #region Commands
    [RelayCommand(CanExecute = nameof(CanCreateOrEdit))]
    private async Task CreateOrEdit()
    {
        University modifiedUniversity;
        var preparedName = Name.Trim();
        var preparedUrl = Url?.Trim();
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
            var doesNameExist = await databaseService.Universities.AnyAsync(uni => uni.Name == preparedName);
            if (doesNameExist)
            {
                await genericDialogService.OpenGenericDialog(UNIVERSITY_NAME_EXISTS_TITLE,
                    UNIVERSITY_NAME_EXISTS_BODY, Constants.GenericStrings.OKAY);
                return;
            }

            var entityEntry = await databaseService.AddAsync(new University
            {
                Name = preparedName,
                Url = preparedUrl
            });
            modifiedUniversity = entityEntry.Entity;
            await Save();
        }

        async Task Update()
        {
            var editingUniversity =
                await databaseService.Universities.FirstOrDefaultAsync(uni => uni.Id == university!.Id);
            if (editingUniversity is null)
            {
                await genericDialogService.OpenGenericDialog(UNIVERSITY_EDITING_NOT_EXIST_TITLE,
                    UNIVERSITY_EDITING_NOT_EXIST_BODY, Constants.GenericStrings.OKAY);
                return;
            }

            var doesNameExist =
                await databaseService.Universities.AnyAsync(uni =>
                    uni.Name == preparedName && uni.Id != editingUniversity.Id);
            if (doesNameExist)
            {
                await genericDialogService.OpenGenericDialog(UNIVERSITY_NAME_EXISTS_TITLE,
                    UNIVERSITY_NAME_EXISTS_BODY, Constants.GenericStrings.OKAY);
                return;
            }

            editingUniversity.Name = preparedName;
            editingUniversity.Url = preparedUrl;
            modifiedUniversity = editingUniversity;
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
            _ = genericDialogService.OpenGenericDialog(UNIVERSITY_FAILED_SAVE_TITLE,
                UNIVERSITY_FAILED_SAVE_BODY, Constants.GenericStrings.OKAY);
        }

        void SaveChangesSuccessHandler(object? sender, SavedChangesEventArgs e)
        {
            Unsubscribe();
            OnRequestCloseWindow?.Invoke(this, new CreateOrEditUniversityEventArgs { University = modifiedUniversity });
        }
    }

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel()
    {
        OnRequestCloseWindow?.Invoke(this, EventArgs.Empty);
    }
    #endregion
}
