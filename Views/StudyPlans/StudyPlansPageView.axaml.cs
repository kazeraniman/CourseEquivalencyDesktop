using CommunityToolkit.Mvvm.DependencyInjection;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.ViewModels.General;
using CourseEquivalencyDesktop.Views.General;

namespace CourseEquivalencyDesktop.Views.StudyPlans;

public partial class StudyPlansPageView : BasePageViewCodeBehind<StudyPlan>
{
    #region Properties
    protected override BasePageView BasePageView => PageRoot;
    #endregion

    #region Constructors
    public StudyPlansPageView()
    {
        InitializeComponent();
    }
    #endregion

    #region BasePageViewCodeBehind
    protected override (BaseCreateOrEditViewModel<StudyPlan>, BaseCreateOrEditWindowCodeBehind<StudyPlan>)
        CreateViewModelAndWindow(StudyPlan? item)
    {
        BaseCreateOrEditViewModel<StudyPlan> viewModel;
        BaseCreateOrEditWindowCodeBehind<StudyPlan> window;
        if (item is null)
        {
            viewModel =
                Ioc.Default.GetRequiredService<ServiceCollectionExtensions.CreateStudyPlanViewModelFactory>()(item);
            window = new CreateStudyPlanWindow
            {
                DataContext = viewModel
            };
        }
        else
        {
            viewModel =
                Ioc.Default.GetRequiredService<ServiceCollectionExtensions.EditStudyPlanViewModelFactory>()(item);
            window = new EditStudyPlanWindow
            {
                DataContext = viewModel
            };
        }

        return (viewModel, window);
    }
    #endregion
}
