using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.Home;
using CourseEquivalencyDesktop.ViewModels.Universities;

namespace CourseEquivalencyDesktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public readonly Interaction<int?, University?> CreateUniversityInteraction = new();

    [ObservableProperty]
    private NavigationPageInfo currentPage;

    [ObservableProperty]
    private bool isLoaded;

    public NavigationPageInfo[] Pages =>
    [
        new NavigationPageInfo("Home", "HomeIconData", new HomePageViewModel()),
        new NavigationPageInfo("Universities", "UniversityIconData", new UniversitiesPageViewModel())
    ];

    public MainWindowViewModel()
    {
        currentPage = Pages[0];
    }

    [RelayCommand]
    private async Task CreateUniversity()
    {
        var universityToCreate = await CreateUniversityInteraction.HandleAsync(null);
        Console.WriteLine(universityToCreate);
    }
}
