using CommunityToolkit.Mvvm.ComponentModel;

namespace CourseEquivalencyDesktop.ViewModels.Universities;

/// <summary>
/// ViewModel which represents a <see cref="CourseEquivalencyDesktop.Models.University"/> to be created.
/// </summary>
public partial class CreateUniversityViewModel : ViewModelBase
{
    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string url;
}
