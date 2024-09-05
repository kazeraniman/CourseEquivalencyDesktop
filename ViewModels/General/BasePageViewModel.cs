using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;

namespace CourseEquivalencyDesktop.ViewModels.General;

public abstract partial class BasePageViewModel<T> : BaseViewModel where T : BaseModel
{
    #region Fields
    public readonly Interaction<T?, T?> CreateOrEditInteraction = new();

    protected readonly ObservableCollection<T> Items = [];
    private readonly DispatcherTimer searchDebounceTimer = new();

    protected readonly DatabaseService DatabaseService;
    private readonly UserSettingsService userSettingsService;
    protected readonly GenericDialogService GenericDialogService;
    #endregion

    #region Properties
    public DataGridCollectionView ItemsCollectionView { get; init; }

    protected abstract string DeleteTitle { get; }
    protected abstract string DeleteFailedTitle { get; }
    protected abstract string DeleteFailedBody { get; }

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
    protected BasePageViewModel()
    {
        Utility.Utility.AssertDesignMode();

        DatabaseService = new DatabaseService();
        userSettingsService = new UserSettingsService();
        GenericDialogService = new GenericDialogService();
        ItemsCollectionView = new DataGridCollectionView(Items);
    }

    protected BasePageViewModel(DatabaseService databaseService, UserSettingsService userSettingsService,
        GenericDialogService genericDialogService)
    {
        DatabaseService = databaseService;
        this.userSettingsService = userSettingsService;
        GenericDialogService = genericDialogService;

        searchDebounceTimer.Tick += SearchDebounce;

        ItemsCollectionView = new DataGridCollectionView(Items)
        {
            Filter = Filter
        };

        ItemsCollectionView.PageChanged += PageChangedHandler;
        Items.CollectionChanged += CollectionChangedHandler;
    }
    #endregion

    #region Finalizer
    ~BasePageViewModel()
    {
        searchDebounceTimer.Stop();
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
        CurrentHumanReadablePageIndex = ItemsCollectionView.PageIndex + 1;
    }

    partial void OnSearchTextChanged(string value)
    {
        searchDebounceTimer.Stop();
        searchDebounceTimer.Start();
    }

    private void SearchDebounce(object? sender, EventArgs eventArgs)
    {
        ItemsCollectionView.Refresh();
        ItemsCollectionView.MoveToFirstPage();
        NextPageCommand.NotifyCanExecuteChanged();
        PreviousPageCommand.NotifyCanExecuteChanged();
        searchDebounceTimer.Stop();
    }
    #endregion

    #region Utility
    private int GetPageCount()
    {
        return (int)Math.Ceiling((double)ItemsCollectionView.TotalItemCount / ItemsCollectionView.PageSize);
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
    protected virtual async Task<T?> CreateInternal()
    {
        var itemToCreate = await CreateOrEditInteraction.HandleAsync(null);
        if (itemToCreate is not null)
        {
            Items.Add(itemToCreate);
        }

        return itemToCreate;
    }

    [RelayCommand]
    private async Task<T?> Create()
    {
        return await CreateInternal();
    }

    [RelayCommand]
    protected async Task<T?> Edit(T item)
    {
        var modifiedItem = await CreateOrEditInteraction.HandleAsync(item);
        if (modifiedItem is not null)
        {
            // This is needed to reload the sort in case the order changed (the values should be correctly propagated without this)
            ItemsCollectionView.Refresh();
        }

        return modifiedItem;
    }

    [RelayCommand]
    private async Task Delete(T item)
    {
        var shouldDelete = await GenericDialogService.OpenGenericDialog(DeleteTitle, GetDeleteBody(item),
            Constants.GenericStrings.DELETE, Constants.GenericStrings.CANCEL,
            primaryButtonThemeName: Constants.ResourceNames.RED_BUTTON);
        if (shouldDelete is null or false)
        {
            return;
        }

        var itemsToRemove = await Remove(item);

        await DatabaseService.SaveChangesAsyncWithCallbacks(SaveChangesSuccessHandler, SaveChangesFailedHandler);

        void SaveChangesSuccessHandler()
        {
            foreach (var itemToRemove in itemsToRemove)
            {
                Items.Remove(itemToRemove);
            }
        }

        void SaveChangesFailedHandler()
        {
            _ = GenericDialogService.OpenGenericDialog(DeleteFailedTitle, DeleteFailedBody,
                Constants.GenericStrings.OKAY);
        }
    }

    [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
    private void NextPage()
    {
        ItemsCollectionView.MoveToNextPage();
    }

    [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
    private void PreviousPage()
    {
        ItemsCollectionView.MoveToPreviousPage();
    }
    #endregion

    #region Overrideable Methods
    public abstract void UpdateItems();
    protected abstract Task<HashSet<T>> Remove(T item);
    protected abstract string GetDeleteBody(T item);
    protected abstract bool Filter(object arg);

    public virtual void ViewLoaded()
    {
        searchDebounceTimer.Interval = userSettingsService.SearchDebounceSecondsTimeSpan;
        ItemsCollectionView.PageSize = userSettingsService.DataGridPageSize;
    }
    #endregion
}
