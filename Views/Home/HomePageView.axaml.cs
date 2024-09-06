using Avalonia.Controls;
using Avalonia.Interactivity;
using CourseEquivalencyDesktop.ViewModels.Home;

namespace CourseEquivalencyDesktop.Views.Home;

public partial class HomePageView : UserControl
{
    #region Constructors
    public HomePageView()
    {
        InitializeComponent();
    }
    #endregion

    #region Avalonia Life Cycle
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (Design.IsDesignMode)
        {
            return;
        }

        if (DataContext is not HomePageViewModel vm)
        {
            return;
        }

        vm.UpdateItems();
    }
    #endregion
}
