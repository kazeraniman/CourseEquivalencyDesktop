using System;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using CourseEquivalencyDesktop.ViewModels.StudyPlans;
using CourseEquivalencyDesktop.Views.General;

namespace CourseEquivalencyDesktop.Views.StudyPlans;

public partial class EditStudyPlanWindow : BaseCreateOrEditWindowCodeBehind
{
    #region Fields
    private IDisposable? requestedCourseEquivalencyInteractionDisposable;
    private IDisposable? addedHomeCourseInteractionDisposable;
    private IDisposable? addedDestinationCourseInteractionDisposable;
    #endregion

    #region Constructors
    public EditStudyPlanWindow()
    {
        InitializeComponent();
    }
    #endregion

    #region Avalonia Life Cycle
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        HomeUniversityCourseAutoCompleteBox.GotFocus +=
            (_, _) => HomeUniversityCourseAutoCompleteBox.IsDropDownOpen = true;
        DestinationUniversityCourseAutoCompleteBox.GotFocus +=
            (_, _) => DestinationUniversityCourseAutoCompleteBox.IsDropDownOpen = true;
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        requestedCourseEquivalencyInteractionDisposable?.Dispose();
        addedHomeCourseInteractionDisposable?.Dispose();
        addedDestinationCourseInteractionDisposable?.Dispose();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        requestedCourseEquivalencyInteractionDisposable?.Dispose();
        addedHomeCourseInteractionDisposable?.Dispose();
        addedDestinationCourseInteractionDisposable?.Dispose();
        if (DataContext is EditStudyPlanViewModel vm)
        {
            requestedCourseEquivalencyInteractionDisposable =
                vm.RequestedCourseEquivalencyInteraction.RegisterHandler(RequestedCourseEquivalencyInteractionHandler);
            addedHomeCourseInteractionDisposable =
                vm.AddedHomeCourseInteraction.RegisterHandler(AddedHomeCourseInteractionHandler);
            addedDestinationCourseInteractionDisposable =
                vm.AddedDestinationCourseInteraction.RegisterHandler(AddedDestinationCourseInteractionHandler);
        }

        base.OnDataContextChanged(e);
    }
    #endregion

    #region Handlers
    private Task<bool?> RequestedCourseEquivalencyInteractionHandler(bool? _)
    {
        DestinationUniversityCourseAutoCompleteBox.Focus();
        return Task.FromResult<bool?>(null);
    }

    private Task<bool?> AddedHomeCourseInteractionHandler(bool? _)
    {
        HomeUniversityCourseAutoCompleteBox.Text = null;
        HomeUniversityCourseAutoCompleteBox.SelectedItem = null;
        return Task.FromResult<bool?>(null);
    }

    private Task<bool?> AddedDestinationCourseInteractionHandler(bool? _)
    {
        DestinationUniversityCourseAutoCompleteBox.Text = null;
        DestinationUniversityCourseAutoCompleteBox.SelectedItem = null;
        return Task.FromResult<bool?>(null);
    }
    #endregion
}
