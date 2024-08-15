using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Views.Universities;

namespace CourseEquivalencyDesktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        SpawnCreateUniversityWindow();
    }

    /// <summary>
    /// Create a window to allow for the creation of a new university.
    /// </summary>
    [RelayCommand]
    private void SpawnCreateUniversityWindow()
    {
        var createUniversityWindow = new CreateUniversityWindow();
        createUniversityWindow.ShowDialog(this);
    }
}
