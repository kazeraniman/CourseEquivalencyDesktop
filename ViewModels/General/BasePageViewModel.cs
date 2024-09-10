using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;

namespace CourseEquivalencyDesktop.ViewModels.General;

public abstract partial class BasePageViewModel<T> : BaseViewModel where T : BaseModel
{
    #region Constants
    private const string DELETE_TITLE_TEMPLATE = "Delete {0}?";
    private const string CREATE_SUCCESSFUL_NOTIFICATION_TITLE = "{0} Created";
    private const string CREATE_SUCCESSFUL_NOTIFICATION_BODY_TEMPLATE = "\"{0}\" was created.";
    private const string EDIT_SUCCESSFUL_NOTIFICATION_TITLE = "{0} Edited";
    private const string EDIT_SUCCESSFUL_NOTIFICATION_BODY_TEMPLATE = "Changes saved for \"{0}\".";
    private const string DELETE_SUCCESSFUL_NOTIFICATION_TITLE = "{0} Deleted";
    private const string DELETE_SUCCESSFUL_NOTIFICATION_BODY_TEMPLATE = "\"{0}\" was deleted.";
    private const string DELETE_FAILED_NOTIFICATION_TITLE = "{0} Delete Failed";

    private const string DELETE_FAILED_NOTIFICATION_BODY_TEMPLATE =
        "An error occurred and \"{0}\" could not be deleted.";
    #endregion

    #region Fields
    public readonly Interaction<T?, T?> CreateOrEditInteraction = new();

    protected readonly ObservableCollection<T> Items = [];
    private readonly DispatcherTimer searchDebounceTimer = new();

    protected readonly DatabaseService DatabaseService = null!;
    private readonly UserSettingsService userSettingsService = null!;
    protected readonly GenericDialogService GenericDialogService = null!;
    protected readonly ToastNotificationService ToastNotificationService = null!;
    #endregion

    #region Properties
    public DataGridCollectionView ItemsCollectionView { get; init; }

    protected abstract string ObjectTypeName { get; }

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

        ItemsCollectionView = new DataGridCollectionView(Items);
    }

    protected BasePageViewModel(DatabaseService databaseService, UserSettingsService userSettingsService,
        GenericDialogService genericDialogService, ToastNotificationService toastNotificationService)
    {
        DatabaseService = databaseService;
        this.userSettingsService = userSettingsService;
        GenericDialogService = genericDialogService;
        ToastNotificationService = toastNotificationService;

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
    protected virtual bool CanCreate()
    {
        return true;
    }

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

    [RelayCommand(CanExecute = nameof(CanCreate))]
    private async Task<T?> Create()
    {
        var createdObject = await CreateInternal();
        if (createdObject is not null)
        {
            ToastNotificationService.ShowToastNotification(
                string.Format(CREATE_SUCCESSFUL_NOTIFICATION_TITLE, ObjectTypeName),
                string.Format(CREATE_SUCCESSFUL_NOTIFICATION_BODY_TEMPLATE, GetName(createdObject)),
                NotificationType.Success);
        }

        return createdObject;
    }

    [RelayCommand]
    protected async Task<T?> Edit(T item)
    {
        var modifiedItem = await CreateOrEditInteraction.HandleAsync(item);
        if (modifiedItem is not null)
        {
            // This is needed to reload the sort in case the order changed (the values should be correctly propagated without this)
            ItemsCollectionView.Refresh();

            ToastNotificationService.ShowToastNotification(
                string.Format(EDIT_SUCCESSFUL_NOTIFICATION_TITLE, ObjectTypeName),
                string.Format(EDIT_SUCCESSFUL_NOTIFICATION_BODY_TEMPLATE, GetName(modifiedItem)),
                NotificationType.Success);
        }

        return modifiedItem;
    }

    [RelayCommand]
    private async Task Delete(T item)
    {
        var shouldDelete = await GenericDialogService.OpenGenericDialog(
            string.Format(DELETE_TITLE_TEMPLATE, ObjectTypeName), GetDeleteBody(item),
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

            ToastNotificationService.ShowToastNotification(
                string.Format(DELETE_SUCCESSFUL_NOTIFICATION_TITLE, ObjectTypeName),
                string.Format(DELETE_SUCCESSFUL_NOTIFICATION_BODY_TEMPLATE, GetName(itemsToRemove.FirstOrDefault())),
                NotificationType.Success);
        }

        void SaveChangesFailedHandler()
        {
            ToastNotificationService.ShowToastNotification(
                string.Format(DELETE_FAILED_NOTIFICATION_TITLE, ObjectTypeName),
                string.Format(DELETE_FAILED_NOTIFICATION_BODY_TEMPLATE, GetName(itemsToRemove.FirstOrDefault())),
                NotificationType.Error);
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
    protected abstract string GetName(T? item);
    protected abstract bool Filter(object arg);

    public virtual void ViewLoaded()
    {
        searchDebounceTimer.Interval = userSettingsService.SearchDebounceSecondsTimeSpan;
        ItemsCollectionView.PageSize = userSettingsService.DataGridPageSize;
    }
    #endregion
}
