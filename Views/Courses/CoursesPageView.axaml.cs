using CommunityToolkit.Mvvm.DependencyInjection;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.ViewModels.General;
using CourseEquivalencyDesktop.Views.General;

namespace CourseEquivalencyDesktop.Views.Courses;

public partial class CoursesPageView : BasePageViewCodeBehind<Course>
{
    #region Properties
    protected override BasePageView BasePageView => PageRoot;
    #endregion

    #region Constructors
    public CoursesPageView()
    {
        InitializeComponent();
    }
    #endregion

    #region BasePageViewCodeBehind
    protected override (BaseCreateOrEditViewModel<Course>, BaseCreateOrEditWindowCodeBehind) CreateViewModelAndWindow(
        Course? item)
    {
        var viewModel =
            Ioc.Default.GetRequiredService<ServiceCollectionExtensions.CreateOrEditCourseViewModelFactory>()(item);
        var window = new CreateOrEditCourseWindow
        {
            DataContext = viewModel
        };

        return (viewModel, window);
    }
    #endregion
}
