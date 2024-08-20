using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Utility;

namespace CourseEquivalencyDesktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public readonly Interaction<int?, University?> CreateUniversityInteraction = new();

    [RelayCommand]
    private async Task CreateUniversity()
    {
        var universityToCreate = await CreateUniversityInteraction.HandleAsync(null);
        Console.WriteLine(universityToCreate);
    }
}
