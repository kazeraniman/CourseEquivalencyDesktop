using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.ViewModels.General;

namespace CourseEquivalencyDesktop.ViewModels.Settings;

public partial class SettingsPageViewModel : BaseViewModel
{
    #region Constants
    private const string SETTINGS_SAVED_TITLE = "Settings Saved";
    private const string SETTINGS_SAVED_BODY = "Your settings have been saved.";
    #endregion

    #region Fields
    private readonly UserSettingsService userSettingsService = null!;
    private readonly ToastNotificationService toastNotificationService = null!;
    #endregion

    #region Properties
    #region Observable Properties
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private bool isSaving;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [Required(AllowEmptyStrings = false)]
    private string? userFullName;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [Required(AllowEmptyStrings = false)]
    private string? userDepartment;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [Required(AllowEmptyStrings = false)]
    [EmailAddress]
    private string? userEmail;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [Required]
    private double searchDelay;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [Required]
    private int dataGridPageSize;
    #endregion
    #endregion

    #region Constructors
    public SettingsPageViewModel()
    {
        Utility.Utility.AssertDesignMode();
    }

    public SettingsPageViewModel(UserSettingsService userSettingsService,
        ToastNotificationService toastNotificationService)
    {
        this.userSettingsService = userSettingsService;
        this.toastNotificationService = toastNotificationService;

        UserFullName = userSettingsService.UserFullName;
        UserDepartment = userSettingsService.UserDepartment;
        UserEmail = userSettingsService.UserEmail;
        SearchDelay = userSettingsService.SearchDebounceSeconds;
        DataGridPageSize = userSettingsService.DataGridPageSize;

        // Ensure the button is disabled if invalid but don't trigger errors as they haven't performed any actions yet
        SaveCommand.NotifyCanExecuteChanged();
    }
    #endregion

    #region Handlers
    partial void OnUserFullNameChanged(string? value)
    {
        ValidateProperty(value, nameof(UserFullName));
    }

    partial void OnUserDepartmentChanged(string? value)
    {
        ValidateProperty(value, nameof(UserDepartment));
    }

    partial void OnUserEmailChanged(string? value)
    {
        ValidateProperty(value, nameof(UserEmail));
    }

    partial void OnSearchDelayChanged(double value)
    {
        ValidateProperty(value, nameof(SearchDelay));
    }

    partial void OnDataGridPageSizeChanged(int value)
    {
        ValidateProperty(value, nameof(DataGridPageSize));
    }
    #endregion

    #region Command Execution Checks
    private bool CanSave()
    {
        return !(IsSaving || HasErrors || string.IsNullOrWhiteSpace(UserFullName) ||
                 string.IsNullOrWhiteSpace(UserDepartment) || string.IsNullOrWhiteSpace(UserEmail));
    }
    #endregion

    #region Commands
    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task Save()
    {
        IsSaving = true;

        var userSettings = userSettingsService.UserSettings;
        userSettings.UserFullName = UserFullName;
        userSettings.UserDepartment = UserDepartment;
        userSettings.UserEmail = UserEmail;
        userSettings.SearchDebounceSeconds = SearchDelay;
        userSettings.DataGridPageSize = DataGridPageSize;

        await userSettingsService.SetAllUserSettings(userSettings);

        toastNotificationService.ShowToastNotification(SETTINGS_SAVED_TITLE, SETTINGS_SAVED_BODY,
            NotificationType.Success);

        IsSaving = false;
    }
    #endregion
}
