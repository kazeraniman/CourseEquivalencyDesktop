using System;
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

public abstract partial class BasePageViewModel<T> : ViewModelBase where T : ModelBase
{
    #region Fields
    public readonly Interaction<T?, T?> CreateOrEditInteraction = new();

    protected readonly ObservableCollection<T> Items = [];
    private readonly DispatcherTimer searchDebounceTimer = new();

    protected readonly DatabaseService DatabaseService;
    private readonly UserSettingsService userSettingsService;
    private readonly GenericDialogService genericDialogService;
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
        genericDialogService = new GenericDialogService();
        ItemsCollectionView = new DataGridCollectionView(Items);
    }

    protected BasePageViewModel(DatabaseService databaseService, UserSettingsService userSettingsService,
        GenericDialogService genericDialogService)
    {
        DatabaseService = databaseService;
        this.userSettingsService = userSettingsService;
        this.genericDialogService = genericDialogService;

        searchDebounceTimer.Interval = userSettingsService.SearchDebounceSecondsTimeSpan;
        searchDebounceTimer.Tick += SearchDebounce;

        ItemsCollectionView = new DataGridCollectionView(Items)
        {
            Filter = Filter,
            PageSize = userSettingsService.DataGridPageSize
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
    [RelayCommand]
    private async Task Create()
    {
        var itemToCreate = await CreateOrEditInteraction.HandleAsync(null);
        if (itemToCreate is not null)
        {
            Items.Add(itemToCreate);
        }
    }

    [RelayCommand]
    private async Task Edit(T item)
    {
        var modifiedItem = await CreateOrEditInteraction.HandleAsync(item);
        if (modifiedItem is not null)
        {
            // This is needed to reload the sort in case the order changed (the values should be correctly propagated without this)
            ItemsCollectionView.Refresh();
        }
    }

    [RelayCommand]
    private async Task Delete(T item)
    {
        var shouldDelete = await genericDialogService.OpenGenericDialog(DeleteTitle, GetDeleteBody(item),
            Constants.GenericStrings.DELETE, Constants.GenericStrings.CANCEL,
            primaryButtonThemeName: Constants.ResourceNames.RED_BUTTON);
        if (shouldDelete is null or false)
        {
            return;
        }

        Remove(item);

        await DatabaseService.SaveChangesAsyncWithCallbacks(SaveChangesSuccessHandler, SaveChangesFailedHandler);

        void SaveChangesSuccessHandler()
        {
            Items.Remove(item);
        }

        void SaveChangesFailedHandler()
        {
            _ = genericDialogService.OpenGenericDialog(DeleteFailedTitle, DeleteFailedBody,
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
    protected abstract void Remove(T item);
    protected abstract string GetDeleteBody(T item);
    protected abstract bool Filter(object arg);
    #endregion
}
