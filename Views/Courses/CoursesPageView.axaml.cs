using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.DependencyInjection;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.ViewModels.Courses;
using CourseEquivalencyDesktop.ViewModels.General;
using CourseEquivalencyDesktop.Views.Equivalencies;
using CourseEquivalencyDesktop.Views.General;

namespace CourseEquivalencyDesktop.Views.Courses;

public partial class CoursesPageView : BasePageViewCodeBehind<Course>
{
    #region Fields
    private IDisposable? createEquivalencyInteractionDisposable;
    #endregion

    #region Avalonia Properties
    public static readonly StyledProperty<bool> ShouldShowHeaderProperty =
        BasePageView.ShouldShowHeaderProperty.AddOwner<CoursesPageView>();

    public static readonly StyledProperty<bool> ShouldShowCreateButtonProperty =
        BasePageView.ShouldShowCreateButtonProperty.AddOwner<CoursesPageView>();
    #endregion

    #region Properties
    protected override BasePageView BasePageView => PageRoot;

    public bool ShouldShowHeader
    {
        get => GetValue(ShouldShowHeaderProperty);
        set => SetValue(ShouldShowHeaderProperty, value);
    }

    public bool ShouldShowCreateButton
    {
        get => GetValue(ShouldShowCreateButtonProperty);
        set => SetValue(ShouldShowCreateButtonProperty, value);
    }
    #endregion

    #region Constructors
    public CoursesPageView()
    {
        InitializeComponent();
    }
    #endregion

    #region Avalonia Life Cycle
    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        createEquivalencyInteractionDisposable?.Dispose();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        createEquivalencyInteractionDisposable?.Dispose();
        if (DataContext is CoursesPageViewModel coursesPageViewModel)
        {
            createEquivalencyInteractionDisposable =
                coursesPageViewModel.CreateEquivalencyInteraction.RegisterHandler(SpawnCreateEquivalencyWindow);
        }

        base.OnDataContextChanged(e);
    }
    #endregion

    #region Interaction Handlers
    private async Task<Course?> SpawnCreateEquivalencyWindow(Course? equivalentCourse)
    {
        if (TopLevel.GetTopLevel(this) is not Window window)
        {
            return null;
        }

        var createEquivalencyViewModel =
            Ioc.Default.GetRequiredService<ServiceCollectionExtensions.CoursesPageViewModelFactory>()(equivalentCourse);
        var createEquivalencyWindow = new CreateEquivalencyWindow
        {
            DataContext = createEquivalencyViewModel
        };

        return await createEquivalencyWindow.ShowDialog<Course?>(window);
    }
    #endregion

    #region BasePageViewCodeBehind
    protected override (BaseCreateOrEditViewModel<Course>, BaseCreateOrEditWindowCodeBehind) CreateViewModelAndWindow(
        Course? item)
    {
        var viewModel =
            Ioc.Default.GetRequiredService<ServiceCollectionExtensions.CreateOrEditCourseViewModelFactory>()(item);
        var window = new CreateOrEditCourseWindow
        {
            DataContext = viewModel
        };

        return (viewModel, window);
    }
    #endregion
}
