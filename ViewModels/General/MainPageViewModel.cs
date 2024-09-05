using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.Courses;
using CourseEquivalencyDesktop.ViewModels.Equivalencies;
using CourseEquivalencyDesktop.ViewModels.Home;
using CourseEquivalencyDesktop.ViewModels.Settings;
using CourseEquivalencyDesktop.ViewModels.Students;
using CourseEquivalencyDesktop.ViewModels.StudyPlans;
using CourseEquivalencyDesktop.ViewModels.Universities;

namespace CourseEquivalencyDesktop.ViewModels.General;

public partial class MainPageViewModel : BaseViewModel
{
    #region Properties
    public NavigationPageInfo[] Pages { get; init; }

    #region Observable Properties
    [ObservableProperty]
    private NavigationPageInfo currentPage;
    #endregion
    #endregion

    #region Constructors
    public MainPageViewModel()
    {
        if (Design.IsDesignMode)
        {
            Pages =
            [
                new NavigationPageInfo("Home", "HomeIconData", new HomePageViewModel()),
                new NavigationPageInfo("Universities", "UniversityIconData", new UniversitiesPageViewModel()),
                new NavigationPageInfo("Courses", "CourseIconData", new CoursesPageViewModel()),
                new NavigationPageInfo("Equivalencies", "EquivalencyIconData", new EquivalenciesPageViewModel()),
                new NavigationPageInfo("Students", "StudentIconData", new StudentsPageViewModel()),
                new NavigationPageInfo("StudyPlans", "StudyPlansIconData", new StudyPlansPageViewModel()),
                new NavigationPageInfo("Settings", "SettingsIconData", new SettingsPageViewModel())
            ];
        }
        else
        {
            Pages =
            [
                new NavigationPageInfo("Home", "HomeIconData", new HomePageViewModel()),
                new NavigationPageInfo("Universities", "UniversityIconData",
                    Ioc.Default.GetRequiredService<UniversitiesPageViewModel>()),
                new NavigationPageInfo("Courses", "CourseIconData",
                    Ioc.Default.GetRequiredService<ServiceCollectionExtensions.CoursesPageViewModelFactory>()(null)),
                new NavigationPageInfo("Equivalencies", "EquivalencyIconData",
                    Ioc.Default.GetRequiredService<EquivalenciesPageViewModel>()),
                new NavigationPageInfo("Students", "StudentIconData",
                    Ioc.Default.GetRequiredService<StudentsPageViewModel>()),
                new NavigationPageInfo("StudyPlans", "StudyPlansIconData",
                    Ioc.Default.GetRequiredService<StudyPlansPageViewModel>()),
                new NavigationPageInfo("Settings", "SettingsIconData",
                    Ioc.Default.GetRequiredService<SettingsPageViewModel>())
            ];
        }

        currentPage = Pages[0];
    }
    #endregion
}
