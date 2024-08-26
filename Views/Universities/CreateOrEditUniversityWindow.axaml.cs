using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CourseEquivalencyDesktop.Views.Universities;

public partial class CreateOrEditUniversityWindow : Window
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
