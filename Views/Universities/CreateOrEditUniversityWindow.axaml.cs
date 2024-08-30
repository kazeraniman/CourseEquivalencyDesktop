using Avalonia.Interactivity;
using CourseEquivalencyDesktop.Views.General;

namespace CourseEquivalencyDesktop.Views.Universities;

public partial class CreateOrEditUniversityWindow : BaseCreateOrEditWindowCodeBehind
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