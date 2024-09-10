using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.ViewModels.StudyPlans;
using CourseEquivalencyDesktop.Views.General;

namespace CourseEquivalencyDesktop.Views.StudyPlans;

public partial class EditStudyPlanWindow : BaseCreateOrEditWindowCodeBehind<StudyPlan>
{
    #region Constants
    private const string DEFAULT_CLASS_NAME = "CourseListItem";
    private const string HAS_EQUIVALENCY_CLASS_NAME = "HasEquivalency";
    private const string NO_EQUIVALENCY_CLASS_NAME = "NoEquivalency";
    private const string HOVERED_EQUIVALENCY_CLASS_NAME = "HoveredEquivalency";
    #endregion

    #region Fields
    private IDisposable? requestedCourseEquivalencyInteractionDisposable;
    private IDisposable? addedHomeCourseInteractionDisposable;
    private IDisposable? addedDestinationCourseInteractionDisposable;

    private Course? hoveredCourse;
    private readonly HashSet<int> hoveredCourseEquivalencyIds = [];
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

        HomeUniversityCoursesItemsRepeater.ElementPrepared += (_, _) => ApplyCourseEquivalencyClasses();
        HomeUniversityCoursesItemsRepeater.ElementClearing += (_, _) => ApplyCourseEquivalencyClasses();
        DestinationUniversityCoursesItemsRepeater.ElementPrepared += (_, _) => ApplyCourseEquivalencyClasses();
        DestinationUniversityCoursesItemsRepeater.ElementClearing += (_, _) => ApplyCourseEquivalencyClasses();

        ApplyCourseEquivalencyClasses();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        ClearDisposables();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        ClearDisposables();
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

    private void CourseListItemPointerEnteredHandler(object? sender, PointerEventArgs e)
    {
        if ((sender as Border)?.Tag is not Course course || hoveredCourse == course)
        {
            return;
        }

        hoveredCourse = course;
        hoveredCourseEquivalencyIds.Clear();
        hoveredCourseEquivalencyIds.UnionWith(hoveredCourse.Equivalencies.Select(ec => ec.Id));
        ApplyCourseEquivalencyClasses();
    }

    private void CourseListItemPointerExitedHandler(object? sender, PointerEventArgs e)
    {
        if (hoveredCourse is null || hoveredCourse != (sender as Border)?.Tag as Course)
        {
            return;
        }

        ClearHoveredCourse();
        ApplyCourseEquivalencyClasses();
    }
    #endregion

    #region Helpers
    private void ClearDisposables()
    {
        requestedCourseEquivalencyInteractionDisposable?.Dispose();
        addedHomeCourseInteractionDisposable?.Dispose();
        addedDestinationCourseInteractionDisposable?.Dispose();
    }

    private void ApplyCourseEquivalencyClasses()
    {
        if (DataContext is not EditStudyPlanViewModel vm)
        {
            return;
        }

        var homeUniversityIdSet = new HashSet<int>(vm.HomeUniversityCourses.Select(huc => huc.Id));
        var destinationUniversityIdSet = new HashSet<int>(vm.DestinationUniversityCourses.Select(duc => duc.Id));
        if (hoveredCourse is not null && !homeUniversityIdSet.Contains(hoveredCourse.Id) &&
            !destinationUniversityIdSet.Contains(hoveredCourse.Id))
        {
            // The hovered course was removed so we want to make sure we clear it out as well since the pointer exit might not fire
            ClearHoveredCourse();
        }

        var homeUniversityEquivalencyIdSet =
            new HashSet<int>(vm.HomeUniversityCourses.SelectMany(huc => huc.Equivalencies.Select(ec => ec.Id)));
        var destinationUniversityEquivalencyIdSet =
            new HashSet<int>(vm.DestinationUniversityCourses.SelectMany(duc => duc.Equivalencies.Select(ec => ec.Id)));
        var isHoveringHomeCourse = hoveredCourse is not null && homeUniversityIdSet.Contains(hoveredCourse.Id);
        var isHoveringDestinationCourse =
            hoveredCourse is not null && destinationUniversityIdSet.Contains(hoveredCourse.Id);

        ApplyClassesToChildren(HomeUniversityCoursesItemsRepeater.Children, destinationUniversityEquivalencyIdSet,
            isHoveringDestinationCourse);
        ApplyClassesToChildren(DestinationUniversityCoursesItemsRepeater.Children, homeUniversityEquivalencyIdSet,
            isHoveringHomeCourse);

        return;

        void ApplyClassesToChildren(Controls controls, HashSet<int> checkedIds, bool doesHoverEquivalencyApply)
        {
            foreach (var child in controls)
            {
                if (child is not Border courseItem)
                {
                    continue;
                }

                if (courseItem.Tag is not Course course)
                {
                    continue;
                }

                courseItem.Classes.Clear();
                courseItem.Classes.Add(DEFAULT_CLASS_NAME);
                courseItem.Classes.Add(checkedIds.Contains(course.Id)
                    ? HAS_EQUIVALENCY_CLASS_NAME
                    : NO_EQUIVALENCY_CLASS_NAME);
                if (course == hoveredCourse ||
                    (doesHoverEquivalencyApply && hoveredCourseEquivalencyIds.Contains(course.Id)))
                {
                    courseItem.Classes.Add(HOVERED_EQUIVALENCY_CLASS_NAME);
                }
            }
        }
    }

    private void ClearHoveredCourse()
    {
        hoveredCourse = null;
        hoveredCourseEquivalencyIds.Clear();
    }
    #endregion
}
