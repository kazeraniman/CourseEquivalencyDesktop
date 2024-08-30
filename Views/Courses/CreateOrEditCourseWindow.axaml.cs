using Avalonia.Interactivity;
using CourseEquivalencyDesktop.Views.General;

namespace CourseEquivalencyDesktop.Views.Courses;

public partial class CreateOrEditCourseWindow : BaseCreateOrEditWindow
{
    #region Constructors
    public CreateOrEditCourseWindow()
    {
        InitializeComponent();
    }
    #endregion

    #region Avalonia Life Cycle
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        UniversityComboBox.Focus();
    }
    #endregion
}
