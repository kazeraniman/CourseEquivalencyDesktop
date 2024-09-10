using Avalonia.Interactivity;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Views.General;

namespace CourseEquivalencyDesktop.Views.StudyPlans;

public partial class CreateStudyPlanWindow : BaseCreateOrEditWindowCodeBehind<StudyPlan>
{
    #region Constructors
    public CreateStudyPlanWindow()
    {
        InitializeComponent();
    }
    #endregion

    #region Avalonia Life Cycle
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        StudentComboBox.Focus();
    }
    #endregion
}
