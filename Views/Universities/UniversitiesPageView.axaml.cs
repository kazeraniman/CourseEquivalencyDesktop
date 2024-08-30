using CommunityToolkit.Mvvm.DependencyInjection;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.ViewModels.General;
using CourseEquivalencyDesktop.Views.General;

namespace CourseEquivalencyDesktop.Views.Universities;

public partial class UniversitiesPageView : BasePageViewCodeBehind<University>
{
    #region Properties
    protected override BasePageView BasePageView => PageRoot;
    #endregion

    #region Constructors
    public UniversitiesPageView()
    {
        InitializeComponent();
    }
    #endregion

    #region BasePageViewCodeBehind
    protected override (BaseCreateOrEditViewModel<University>, BaseCreateOrEditWindow) CreateViewModelAndWindow(
        University? item)
    {
        var viewModel =
            Ioc.Default.GetRequiredService<ServiceCollectionExtensions.CreateOrEditUniversityViewModelFactory>()(item);
        var window = new CreateOrEditUniversityWindow
        {
            DataContext = viewModel
        };

        return (viewModel, window);
    }
    #endregion
}
