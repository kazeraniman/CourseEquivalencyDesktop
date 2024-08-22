﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.DependencyInjection;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.ViewModels.Universities;

namespace CourseEquivalencyDesktop.Views.Universities;

public partial class UniversitiesPageView : UserControl
{
    private IDisposable? createUniversityInteractionDisposable;

    public UniversitiesPageView()
    {
        InitializeComponent();
    }

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

        universitiesPageViewModel.UpdateUniversities();
        UniversitiesDataGrid.Columns[0].Sort(ListSortDirection.Ascending);
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
            createUniversityInteractionDisposable = vm.CreateOrEditUniversityInteraction.RegisterHandler(SpawnCreateUniversityWindow);
        }

        base.OnDataContextChanged(e);
    }

    private async Task<University?> SpawnCreateUniversityWindow(University? university)
    {
        if (TopLevel.GetTopLevel(this) is not Window window)
        {
            return null;
        }

        var createUniversityViewModel = Ioc.Default.GetRequiredService<ServiceCollectionExtensions.CreateOrEditUniversityViewModelFactory>()(university);
        var createUniversityWindow = new CreateOrEditUniversityWindow
        {
            DataContext = createUniversityViewModel
        };

        createUniversityViewModel.OnRequestCloseWindow += (_, args) => createUniversityWindow.Close((args as CreateOrEditUniversityViewModel.CreateOrEditUniversityEventArgs)?.University);

        return await createUniversityWindow.ShowDialog<University>(window);
    }
}
