using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.Home;
using CourseEquivalencyDesktop.ViewModels.Universities;

namespace CourseEquivalencyDesktop.ViewModels.General;

public partial class MainPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private NavigationPageInfo currentPage;

    public NavigationPageInfo[] Pages { get; init; }

    public MainPageViewModel()
    {
        if (Design.IsDesignMode)
        {
            Pages =
            [
                new NavigationPageInfo("Home", "HomeIconData", new HomePageViewModel()),
                new NavigationPageInfo("Universities", "UniversityIconData", new UniversitiesPageViewModel())
            ];
        }
        else
        {
            Pages =
            [
                new NavigationPageInfo("Home", "HomeIconData", new HomePageViewModel()),
                new NavigationPageInfo("Universities", "UniversityIconData", Ioc.Default.GetRequiredService<UniversitiesPageViewModel>())
            ];
        }

        currentPage = Pages[0];
    }
}
