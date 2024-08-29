using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.DependencyInjection;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.ViewModels.General;
using CourseEquivalencyDesktop.ViewModels.Universities;

namespace CourseEquivalencyDesktop.Views.Universities;

public partial class UniversitiesPageView : UserControl
{
    #region Fields
    private IDisposable? createUniversityInteractionDisposable;
    #endregion

    #region Constructors
    public UniversitiesPageView()
    {
        InitializeComponent();
    }
    #endregion

    #region Avalonia Life Cycle
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is not UniversitiesPageViewModel universitiesPageViewModel)
        {
            return;
        }

        if (Design.IsDesignMode)
        {
            return;
        }

        universitiesPageViewModel.UpdateItems();
        PageRoot.ApplyInitialSort();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        createUniversityInteractionDisposable?.Dispose();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        createUniversityInteractionDisposable?.Dispose();
        if (DataContext is UniversitiesPageViewModel vm)
        {
            createUniversityInteractionDisposable =
                vm.CreateOrEditInteraction.RegisterHandler(SpawnCreateUniversityWindow);
        }

        base.OnDataContextChanged(e);
    }
    #endregion

    #region Interaction Handlers
    private async Task<University?> SpawnCreateUniversityWindow(University? university)
    {
        if (TopLevel.GetTopLevel(this) is not Window window)
        {
            return null;
        }

        var createOrEditUniversityViewModel =
            Ioc.Default.GetRequiredService<ServiceCollectionExtensions.CreateOrEditUniversityViewModelFactory>()(
                university);
        var createOrEditUniversityWindow = new CreateOrEditUniversityWindow
        {
            DataContext = createOrEditUniversityViewModel
        };

        createOrEditUniversityViewModel.OnRequestCloseWindow += (_, args) =>
            createOrEditUniversityWindow.Close((args as BaseCreateOrEditViewModel<University>.CreateOrEditEventArgs)
                ?.Item);

        return await createOrEditUniversityWindow.ShowDialog<University>(window);
    }
    #endregion
}
