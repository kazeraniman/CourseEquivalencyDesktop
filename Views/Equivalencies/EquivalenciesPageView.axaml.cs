using System;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.ViewModels.General;
using CourseEquivalencyDesktop.Views.General;

namespace CourseEquivalencyDesktop.Views.Equivalencies;

public partial class EquivalenciesPageView : BasePageViewCodeBehind<CourseEquivalency>
{
    #region Properties
    protected override BasePageView BasePageView => PageRoot;
    #endregion

    #region Constructors
    public EquivalenciesPageView()
    {
        InitializeComponent();
    }
    #endregion


    #region BasePageViewCodeBehind
    protected override (BaseCreateOrEditViewModel<CourseEquivalency>,
        BaseCreateOrEditWindowCodeBehind<CourseEquivalency>)
        CreateViewModelAndWindow(CourseEquivalency? item)
    {
        throw new NotImplementedException();
    }
    #endregion
}
