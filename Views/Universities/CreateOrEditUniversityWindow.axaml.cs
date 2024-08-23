using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CourseEquivalencyDesktop.Views.Universities;

public partial class CreateOrEditUniversityWindow : Window
{
    public CreateOrEditUniversityWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        UniversityTextBox.Focus();
    }
}
