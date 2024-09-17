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
    #region Constants
    private const string HOME_LABEL = "Home";
    private const string UNIVERSITIES_LABEL = "Universities";
    private const string COURSES_LABEL = "Courses";
    private const string EQUIVALENCIES_LABEL = "Equivalencies";
    private const string STUDENTS_LABEL = "Students";
    private const string STUDY_PLANS_LABEL = "Study Plans";
    private const string SETTINGS_LABEL = "Settings";
    private const string HOME_ICON = "HomeIconData";
    private const string UNIVERSITY_ICON = "UniversityIconData";
    private const string COURSES_ICON = "CourseIconData";
    private const string EQUIVALENCIES_ICON = "EquivalencyIconData";
    private const string STUDENTS_ICON = "StudentIconData";
    private const string STUDY_PLANS_ICON = "StudyPlansIconData";
    private const string SETTINGS_ICON = "SettingsIconData";
    #endregion

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
                new NavigationPageInfo(HOME_LABEL, HOME_ICON, new HomePageViewModel()),
                new NavigationPageInfo(UNIVERSITIES_LABEL, UNIVERSITY_ICON, new UniversitiesPageViewModel()),
                new NavigationPageInfo(COURSES_LABEL, COURSES_ICON, new CoursesPageViewModel()),
                new NavigationPageInfo(EQUIVALENCIES_LABEL, EQUIVALENCIES_ICON, new EquivalenciesPageViewModel()),
                new NavigationPageInfo(STUDENTS_LABEL, STUDENTS_ICON, new StudentsPageViewModel()),
                new NavigationPageInfo(STUDY_PLANS_LABEL, STUDY_PLANS_ICON, new StudyPlansPageViewModel()),
                new NavigationPageInfo(SETTINGS_LABEL, SETTINGS_ICON, new SettingsPageViewModel())
            ];
        }
        else
        {
            Pages =
            [
                new NavigationPageInfo(HOME_LABEL, HOME_ICON, Ioc.Default.GetRequiredService<HomePageViewModel>()),
                new NavigationPageInfo(UNIVERSITIES_LABEL, UNIVERSITY_ICON,
                    Ioc.Default.GetRequiredService<UniversitiesPageViewModel>()),
                new NavigationPageInfo(COURSES_LABEL, COURSES_ICON,
                    Ioc.Default.GetRequiredService<ServiceCollectionExtensions.CoursesPageViewModelFactory>()(null)),
                new NavigationPageInfo(EQUIVALENCIES_LABEL, EQUIVALENCIES_ICON,
                    Ioc.Default.GetRequiredService<EquivalenciesPageViewModel>()),
                new NavigationPageInfo(STUDENTS_LABEL, STUDENTS_ICON,
                    Ioc.Default.GetRequiredService<StudentsPageViewModel>()),
                new NavigationPageInfo(STUDY_PLANS_LABEL, STUDY_PLANS_ICON,
                    Ioc.Default.GetRequiredService<StudyPlansPageViewModel>()),
                new NavigationPageInfo(SETTINGS_LABEL, SETTINGS_ICON,
                    Ioc.Default.GetRequiredService<SettingsPageViewModel>())
            ];
        }

        currentPage = Pages[0];
    }
    #endregion
}
