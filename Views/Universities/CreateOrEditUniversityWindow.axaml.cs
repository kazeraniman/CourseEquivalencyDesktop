using Avalonia.Interactivity;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Views.General;

namespace CourseEquivalencyDesktop.Views.Universities;

public partial class CreateOrEditUniversityWindow : BaseCreateOrEditWindowCodeBehind<University>
{
    #region Constructors
    public CreateOrEditUniversityWindow()
    {
        InitializeComponent();
    }
    #endregion

    #region Avalonia Life Cycle
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        UniversityTextBox.Focus();
    }
    #endregion
}
