using Avalonia.Controls;
using Avalonia.Interactivity;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.ViewModels.General;

namespace CourseEquivalencyDesktop.Views.General;

public abstract class BaseCreateOrEditWindowCodeBehind<T> : Window where T : BaseModel
{
    #region Avalonia Life Cycle
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is not BaseCreateOrEditViewModel<T> vm)
        {
            return;
        }

        var windowRoot = this.Find<BaseCreateOrEditWindow>("WindowRoot");
        vm.SetUpNotificationsCommand.Execute(windowRoot?.GetWindowNotificationManager());
    }
    #endregion
}
