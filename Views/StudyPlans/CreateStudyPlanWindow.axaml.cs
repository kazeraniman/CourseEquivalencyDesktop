using Avalonia.Interactivity;
using CourseEquivalencyDesktop.Views.General;

namespace CourseEquivalencyDesktop.Views.StudyPlans;

public partial class CreateStudyPlanWindow : BaseCreateOrEditWindowCodeBehind
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
