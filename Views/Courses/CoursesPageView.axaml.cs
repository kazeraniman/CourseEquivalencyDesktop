﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.DependencyInjection;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.ViewModels.Courses;

namespace CourseEquivalencyDesktop.Views.Courses;

public partial class CoursesPageView : UserControl
{
    #region Fields
    private IDisposable? createCourseInteractionDisposable;
    #endregion

    #region Constructors
    public CoursesPageView()
    {
        InitializeComponent();
    }
    #endregion

    #region Avalonia Life Cycle
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is not CoursesPageViewModel coursesPageViewModel)
        {
            return;
        }

        if (Design.IsDesignMode)
        {
            return;
        }

        CoursesDataGrid.Columns[1].Sort(ListSortDirection.Ascending); // TODO: Why doesn't this work?
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        createCourseInteractionDisposable?.Dispose();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        createCourseInteractionDisposable?.Dispose();
        if (DataContext is CoursesPageViewModel vm)
        {
            createCourseInteractionDisposable =
                vm.CreateOrEditCourseInteraction.RegisterHandler(SpawnCreateUniversityWindow);
        }

        base.OnDataContextChanged(e);
    }
    #endregion

    #region Interaction Handlers
    private async Task<Course?> SpawnCreateUniversityWindow(Course? course)
    {
        if (TopLevel.GetTopLevel(this) is not Window window)
        {
            return null;
        }

        var createOrEditCourseViewModel =
            Ioc.Default.GetRequiredService<ServiceCollectionExtensions.CreateOrEditCourseViewModelFactory>()(
                course);
        var createOrEditCourseWindow = new CreateOrEditCourseWindow
        {
            DataContext = createOrEditCourseViewModel
        };

        createOrEditCourseViewModel.OnRequestCloseWindow += (_, args) =>
            createOrEditCourseWindow.Close((args as CreateOrEditCourseViewModel.CreateOrEditCourseEventArgs)
                ?.Course);

        return await createOrEditCourseWindow.ShowDialog<Course>(window);
    }
    #endregion
}
