using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.ViewModels.Universities;

namespace CourseEquivalencyDesktop.Views.Universities;

public partial class UniversitiesPageView : UserControl
{
    private IDisposable? createUniversityInteractionDisposable;

    public UniversitiesPageView()
    {
        InitializeComponent();
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
            createUniversityInteractionDisposable = vm.CreateUniversityInteraction.RegisterHandler(SpawnCreateUniversityWindow);
        }

        base.OnDataContextChanged(e);
    }

    private async Task<University?> SpawnCreateUniversityWindow(int? id)
    {
        if (TopLevel.GetTopLevel(this) is not Window window)
        {
            return null;
        }

        var createUniversityViewModel = new CreateUniversityViewModel();
        var createUniversityWindow = new CreateUniversityWindow
        {
            DataContext = createUniversityViewModel
        };

        createUniversityViewModel.OnRequestCloseWindow += (_, args) => createUniversityWindow.Close((args as CreateUniversityViewModel.CreateUniversityEventArgs)?.University);

        return await createUniversityWindow.ShowDialog<University>(window);
    }
}
