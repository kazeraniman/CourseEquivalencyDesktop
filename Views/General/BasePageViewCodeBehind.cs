using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.ViewModels.General;

namespace CourseEquivalencyDesktop.Views.General;

public abstract class BasePageViewCodeBehind<T> : UserControl where T : BaseModel
{
    #region Fields
    private IDisposable? createInteractionDisposable;
    #endregion

    #region Properties
    protected abstract BasePageView BasePageView { get; }
    #endregion

    #region Avalonia Life Cycle
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is not BasePageViewModel<T> basePageViewModel)
        {
            return;
        }

        if (Design.IsDesignMode)
        {
            return;
        }

        basePageViewModel.UpdateItems();
        BasePageView.ApplyInitialSort();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        createInteractionDisposable?.Dispose();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        createInteractionDisposable?.Dispose();
        if (DataContext is BasePageViewModel<T> basePageViewModel)
        {
            createInteractionDisposable =
                basePageViewModel.CreateOrEditInteraction.RegisterHandler(SpawnCreateOrEditWindow);
        }

        base.OnDataContextChanged(e);
    }
    #endregion

    #region Interaction Handlers
    private async Task<T?> SpawnCreateOrEditWindow(T? item)
    {
        if (TopLevel.GetTopLevel(this) is not Window window)
        {
            return null;
        }

        var (createOrEditViewModel, createOrEditWindow) = CreateViewModelAndWindow(item);

        createOrEditViewModel.OnRequestCloseWindow += (_, args) =>
            createOrEditWindow.Close((args as BaseCreateOrEditViewModel<T>.CreateOrEditEventArgs)?.Item);

        return await createOrEditWindow.ShowDialog<T>(window);
    }
    #endregion

    #region Overrideable Methods
    protected abstract (BaseCreateOrEditViewModel<T>, BaseCreateOrEditWindowCodeBehind)
        CreateViewModelAndWindow(T? item);
    #endregion
}