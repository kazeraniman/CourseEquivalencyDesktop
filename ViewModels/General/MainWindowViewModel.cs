using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Utility;

namespace CourseEquivalencyDesktop.ViewModels.General;

public partial class MainWindowViewModel : ViewModelBase
{
    public readonly Interaction<int?, University?> CreateUniversityInteraction = new();

    [ObservableProperty]
    private ViewModelBase currentContent = new MainPageLoadingViewModel();

    public void CompleteLoad()
    {
        CurrentContent = new MainPageViewModel();
    }

    [RelayCommand]
    private async Task CreateUniversity()
    {
        var universityToCreate = await CreateUniversityInteraction.HandleAsync(null);
        Console.WriteLine(universityToCreate);
    }
}
