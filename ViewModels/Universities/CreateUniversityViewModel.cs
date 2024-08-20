using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using Microsoft.EntityFrameworkCore;

namespace CourseEquivalencyDesktop.ViewModels.Universities;

/// <summary>
/// ViewModel which represents a <see cref="CourseEquivalencyDesktop.Models.University"/> to be created.
/// </summary>
public partial class CreateUniversityViewModel : ViewModelBase
{
    public class CreateUniversityEventArgs : EventArgs
    {
        public University? University { get; init; }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    [Required]
    private string name = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    [Url]
    private string? url;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private bool isCreating;

    public event EventHandler? OnRequestCloseWindow;

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

    private bool CanCreate()
    {
        return !(IsCreating || HasErrors || string.IsNullOrWhiteSpace(Name));
    }

    [RelayCommand(CanExecute = nameof(CanCreate))]
    private async Task Create()
    {
        var preparedName = Name.Trim();
        var preparedUrl = Url?.Trim();

        var databaseService = Ioc.Default.GetRequiredService<DatabaseService>();
        if (databaseService.Universities.Any(university => university.Name == preparedName))
        {
            // TODO: Proper error handling dialog box
            Console.WriteLine("A university with this name already exists.");
            return;
        }

        var entityEntry = databaseService.Add(new University
        {
            Name = preparedName,
            Url = preparedUrl
        });
        databaseService.SaveChangesFailed += SaveChangesFailedHandler;
        databaseService.SavedChanges += SaveChangesSuccessHandler;
        await databaseService.SaveChangesAsync();

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
            OnRequestCloseWindow?.Invoke(this, new CreateUniversityEventArgs { University = entityEntry.Entity });
        }
    }

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel()
    {
        OnRequestCloseWindow?.Invoke(this, EventArgs.Empty);
    }
}
