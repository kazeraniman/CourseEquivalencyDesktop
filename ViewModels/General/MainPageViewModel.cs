using CommunityToolkit.Mvvm.ComponentModel;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.Home;
using CourseEquivalencyDesktop.ViewModels.Universities;

namespace CourseEquivalencyDesktop.ViewModels.General;

public partial class MainPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private NavigationPageInfo currentPage;

    public NavigationPageInfo[] Pages =>
    [
        new NavigationPageInfo("Home", "HomeIconData", new HomePageViewModel()),
        new NavigationPageInfo("Universities", "UniversityIconData", new UniversitiesPageViewModel())
    ];

    public MainPageViewModel()
    {
        currentPage = Pages[0];
    }
}
