using Avalonia.Interactivity;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Views.General;

namespace CourseEquivalencyDesktop.Views.Students;

public partial class CreateOrEditStudentWindow : BaseCreateOrEditWindowCodeBehind<Student>
{
    #region Constructors
    public CreateOrEditStudentWindow()
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
