using CommunityToolkit.Mvvm.DependencyInjection;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.ViewModels.General;
using CourseEquivalencyDesktop.Views.General;

namespace CourseEquivalencyDesktop.Views.Students;

public partial class StudentsPageView : BasePageViewCodeBehind<Student>
{
    #region Properties
    protected override BasePageView BasePageView => PageRoot;
    #endregion

    #region Constructors
    public StudentsPageView()
    {
        InitializeComponent();
    }
    #endregion

    #region BasePageViewCodeBehind
    protected override (BaseCreateOrEditViewModel<Student>, BaseCreateOrEditWindowCodeBehind) CreateViewModelAndWindow(
        Student? item)
    {
        var viewModel =
            Ioc.Default.GetRequiredService<ServiceCollectionExtensions.CreateOrEditStudentViewModelFactory>()(item);
        var window = new CreateOrEditStudentWindow
        {
            DataContext = viewModel
        };

        return (viewModel, window);
    }
    #endregion
}
