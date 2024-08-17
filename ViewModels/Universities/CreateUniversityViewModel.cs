using System;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;

namespace CourseEquivalencyDesktop.ViewModels.Universities;

/// <summary>
/// ViewModel which represents a <see cref="CourseEquivalencyDesktop.Models.University"/> to be created.
/// </summary>
public partial class CreateUniversityViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    [Required]
    private string name;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    [Url]
    private string url;

    public event EventHandler OnRequestCloseWindow;

    public University GetUniversity => new()
    {
        Name = Name,
        Url = Url
    };

    partial void OnUrlChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            ClearErrors(nameof(Url));
            return;
        }

        ValidateProperty(value, nameof(Url));
    }

    private bool CanCreate()
    {
        return !(HasErrors || string.IsNullOrWhiteSpace(Name));
    }

    [RelayCommand(CanExecute = nameof(CanCreate))]
    private void Create()
    {
        // TODO: Actually perform the necessary database actions (creation, ensuring there aren't dupes, etc.)
        Console.WriteLine(GetUniversity);
        OnRequestCloseWindow(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Cancel()
    {
        OnRequestCloseWindow(this, EventArgs.Empty);
    }
}
