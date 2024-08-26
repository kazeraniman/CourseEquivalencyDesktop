﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;
using Microsoft.EntityFrameworkCore;

namespace CourseEquivalencyDesktop.ViewModels.Universities;

public partial class UniversitiesPageViewModel : ViewModelBase
{
    #region Fields
    public readonly Interaction<University?, University?> CreateOrEditUniversityInteraction = new();

    private readonly ObservableCollection<University> universities = [];

    private readonly DatabaseService databaseService;
    private readonly UserSettingsService userSettingsService;
    #endregion

    #region Properties
    public DataGridCollectionView UniversitiesCollectionView { get; init; }

    #region Observable Properties
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    private string searchText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    private int currentHumanReadablePageIndex = 1;
    #endregion
    #endregion

    #region Constructors
    public UniversitiesPageViewModel()
    {
        Utility.Utility.AssertDesignMode();

        databaseService = new DatabaseService();
        userSettingsService = new UserSettingsService();
        UniversitiesCollectionView = new DataGridCollectionView(universities);
    }

    public UniversitiesPageViewModel(DatabaseService databaseService, UserSettingsService userSettingsService)
    {
        this.databaseService = databaseService;
        this.userSettingsService = userSettingsService;

        UniversitiesCollectionView = new DataGridCollectionView(universities)
        {
            Filter = Filter,
            PageSize = userSettingsService.DataGridPageSize
        };

        UniversitiesCollectionView.PageChanged += PageChangedHandler;
        universities.CollectionChanged += CollectionChangedHandler;
    }
    #endregion

    #region Handlers
    private void CollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e)
    {
        PreviousPageCommand.NotifyCanExecuteChanged();
        NextPageCommand.NotifyCanExecuteChanged();
    }

    private void PageChangedHandler(object? sender, EventArgs e)
    {
        CurrentHumanReadablePageIndex = UniversitiesCollectionView.PageIndex + 1;
    }

    partial void OnSearchTextChanged(string value)
    {
        // TODO: Debounce so this doesn't constantly happen
        UniversitiesCollectionView.Refresh();
        UniversitiesCollectionView.MoveToFirstPage();
    }

    private bool Filter(object arg)
    {
        if (arg is not University university)
        {
            return false;
        }

        return string.IsNullOrWhiteSpace(SearchText) || university.Name.CaseInsensitiveContains(SearchText) ||
               (university.Url?.CaseInsensitiveContains(SearchText) ?? false);
    }
    #endregion

    #region Utility
    public void UpdateUniversities()
    {
        universities.Clear();
        universities.AddRange(databaseService.Universities);
    }

    private int GetPageCount()
    {
        return (int)Math.Ceiling(
            (double)UniversitiesCollectionView.TotalItemCount / UniversitiesCollectionView.PageSize);
    }
    #endregion

    #region Command Execution Checks
    private bool CanGoToNextPage()
    {
        return GetPageCount() > CurrentHumanReadablePageIndex;
    }

    private bool CanGoToPreviousPage()
    {
        return CurrentHumanReadablePageIndex > 1;
    }
    #endregion

    #region Commands
    [RelayCommand]
    private async Task CreateUniversity()
    {
        var universityToCreate = await CreateOrEditUniversityInteraction.HandleAsync(null);
        if (universityToCreate is not null)
        {
            universities.Add(universityToCreate);
        }
    }

    [RelayCommand]
    private async Task EditUniversity(University university)
    {
        var editedUniversity = await CreateOrEditUniversityInteraction.HandleAsync(university);
        if (editedUniversity is not null)
        {
            UniversitiesCollectionView.Refresh();
        }
    }

    [RelayCommand]
    private async Task DeleteUniversity(University university)
    {
        // TODO: Confirmation Dialog
        databaseService.Universities.Remove(university);
        databaseService.SaveChangesFailed += SaveChangesFailedHandler;
        databaseService.SavedChanges += SaveChangesSuccessHandler;
        await databaseService.SaveChangesAsync();

        void Unsubscribe()
        {
            databaseService.SaveChangesFailed -= SaveChangesFailedHandler;
            databaseService.SavedChanges -= SaveChangesSuccessHandler;
        }

        void SaveChangesFailedHandler(object? sender, SaveChangesFailedEventArgs e)
        {
            Unsubscribe();
            // TODO: Proper error handling dialog box
            Console.WriteLine("Failed to delete university.");
        }

        void SaveChangesSuccessHandler(object? sender, SavedChangesEventArgs e)
        {
            Unsubscribe();
            universities.Remove(university);
        }
    }

    [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
    private void NextPage()
    {
        UniversitiesCollectionView.MoveToNextPage();
    }

    [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
    private void PreviousPage()
    {
        UniversitiesCollectionView.MoveToPreviousPage();
    }
    #endregion
}
