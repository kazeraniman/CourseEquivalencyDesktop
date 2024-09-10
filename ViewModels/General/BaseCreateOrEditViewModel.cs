using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;

namespace CourseEquivalencyDesktop.ViewModels.General;

public abstract partial class BaseCreateOrEditViewModel<T> : BaseViewModel where T : BaseModel
{
    public class CreateOrEditEventArgs : EventArgs
    {
        public T? Item { get; init; }
    }

    #region Fields
    private WindowNotificationManager? windowNotificationManager;

    protected readonly DatabaseService DatabaseService = null!;

    protected readonly T? Item;
    protected readonly bool IsCreate;
    #endregion

    #region Properties
    protected abstract string FailedSaveTitle { get; }
    protected abstract string FailedSaveBody { get; }

    #region Observable Properties
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateOrEditCommand), nameof(CancelCommand))]
    private bool isCreating;

    [ObservableProperty]
    // ReSharper disable once InconsistentNaming
    private string windowAndButtonText = "INHERITED, PLEASE OVERRIDE";
    #endregion
    #endregion

    #region Events
    public event EventHandler? OnRequestCloseWindow;
    #endregion

    #region Constructors
    protected BaseCreateOrEditViewModel()
    {
        Utility.Utility.AssertDesignMode();
    }

    protected BaseCreateOrEditViewModel(T? item, DatabaseService databaseService)
    {
        DatabaseService = databaseService;
        Item = item;
        IsCreate = item is null;
    }
    #endregion

    #region Utility
    protected void CallOnRequestCloseWindow(T? modifiedItem)
    {
        OnRequestCloseWindow?.Invoke(this, new CreateOrEditEventArgs { Item = modifiedItem });
    }

    protected async Task SaveChanges(T? modifiedItem)
    {
        await DatabaseService.SaveChangesAsyncWithCallbacks(SaveChangesSuccessHandler, SaveChangesFailedHandler);

        void SaveChangesSuccessHandler()
        {
            CallOnRequestCloseWindow(modifiedItem);
        }

        void SaveChangesFailedHandler()
        {
            ShowNotification(FailedSaveTitle, FailedSaveBody, NotificationType.Error);
        }
    }

    protected void ShowNotification(string title, string message, NotificationType notificationType,
        TimeSpan? duration = null)
    {
        ToastNotificationService.ShowToastNotification(windowNotificationManager, title, message, notificationType,
            duration);
    }
    #endregion

    #region Command Execution Checks
    private bool CanCancel()
    {
        return !IsCreating;
    }

    protected virtual bool CanCreateOrEdit()
    {
        return !(IsCreating || HasErrors);
    }
    #endregion

    #region Commands
    protected abstract Task CreateOrEditInherited();

    [RelayCommand(CanExecute = nameof(CanCreateOrEdit))]
    private async Task CreateOrEdit()
    {
        await CreateOrEditInherited();
    }

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel()
    {
        OnRequestCloseWindow?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void SetUpNotifications(WindowNotificationManager? notificationManager)
    {
        windowNotificationManager = notificationManager;
    }
    #endregion
}
