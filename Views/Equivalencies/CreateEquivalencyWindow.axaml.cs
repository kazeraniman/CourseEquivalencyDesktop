using Avalonia.Controls;
using Avalonia.Interactivity;
using CourseEquivalencyDesktop.ViewModels.Courses;

namespace CourseEquivalencyDesktop.Views.Equivalencies;

public partial class CreateEquivalencyWindow : Window
{
    #region Constructors
    public CreateEquivalencyWindow()
    {
        InitializeComponent();
    }
    #endregion

    #region Avalonia Life Cycle
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is not CoursesPageViewModel vm)
        {
            return;
        }

        vm.SetUpEquivalencyNotificationsCommand.Execute(CreateEquivalenciesWindowNotificationManager);
    }
    #endregion
}
