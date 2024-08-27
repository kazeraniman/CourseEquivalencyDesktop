using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CourseEquivalencyDesktop.Views.Courses;

public partial class CreateOrEditCourseWindow : Window
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
