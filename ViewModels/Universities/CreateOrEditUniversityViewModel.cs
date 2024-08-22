using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.ViewModels.General;
using Microsoft.EntityFrameworkCore;

namespace CourseEquivalencyDesktop.ViewModels.Universities;

/// <summary>
/// ViewModel which represents a <see cref="CourseEquivalencyDesktop.Models.University"/> to be created.
/// </summary>
public partial class CreateOrEditUniversityViewModel : ViewModelBase
{
    public class CreateOrEditUniversityEventArgs : EventArgs
    {
        public University? University { get; init; }
    }

    private const string CREATE_TEXT = "Create University";
    private const string EDIT_TEXT = "Edit University";

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

    public event EventHandler? OnRequestCloseWindow;

    private readonly DatabaseService databaseService;
    private readonly University? university;
    private readonly bool isCreate;

    public CreateOrEditUniversityViewModel()
    {
        Utility.Utility.AssertDesignMode();

        databaseService = new DatabaseService();
        WindowAndButtonText = CREATE_TEXT;
    }

    public CreateOrEditUniversityViewModel(University? university, DatabaseService databaseService)
    {
        this.databaseService = databaseService;
        this.university = university;
        isCreate = university is null;
        WindowAndButtonText = isCreate ? CREATE_TEXT : EDIT_TEXT;

        Name = university?.Name ?? string.Empty;
        Url = university?.Url;
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

    private bool CanCancel()
    {
        return !IsCreating;
    }

    private bool CanCreateOrEdit()
    {
        return !(IsCreating || HasErrors || string.IsNullOrWhiteSpace(Name));
    }

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
                // TODO: Proper error handling dialog box
                Console.WriteLine("A university with this name already exists.");
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
            var editingUniversity = await databaseService.Universities.FirstOrDefaultAsync(uni => uni.Id == university!.Id);
            if (editingUniversity is null)
            {
                // TODO: Proper error handling dialog box
                Console.WriteLine("Editing a university which does not exist.");
                return;
            }

            var doesNameExist = await databaseService.Universities.AnyAsync(uni => uni.Name == preparedName && uni.Id != editingUniversity.Id);
            if (doesNameExist)
            {
                // TODO: Proper error handling dialog box
                Console.WriteLine("A university with this name already exists.");
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
            // TODO: Proper error handling dialog box
            Console.WriteLine("Failed to create university.");
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
}
