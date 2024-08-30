using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;

namespace CourseEquivalencyDesktop.ViewModels.General;

public abstract partial class BaseCreateOrEditViewModel<T> : ViewModelBase where T : BaseModel
{
    public class CreateOrEditEventArgs : EventArgs
    {
        public T? Item { get; init; }
    }

    #region Fields
    protected readonly DatabaseService DatabaseService;
    protected readonly GenericDialogService GenericDialogService;

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

        DatabaseService = new DatabaseService();
        GenericDialogService = new GenericDialogService();
    }

    protected BaseCreateOrEditViewModel(T? item, DatabaseService databaseService,
        GenericDialogService genericDialogService)
    {
        DatabaseService = databaseService;
        GenericDialogService = genericDialogService;
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
            _ = GenericDialogService.OpenGenericDialog(FailedSaveTitle, FailedSaveBody,
                Constants.GenericStrings.OKAY);
        }
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
    #endregion
}