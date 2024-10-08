﻿using System.ComponentModel.DataAnnotations;
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
    private const string SELECT_TEMPLATE_DIALOG_TITLE = "Select Template";
    #endregion

    #region Fields
    private readonly UserSettingsService userSettingsService = null!;
    private readonly ToastNotificationService toastNotificationService = null!;
    private readonly FileDialogService fileDialogService = null!;
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
    [Required(AllowEmptyStrings = false)]
    private string? creditTransferMemoTemplateFilePath;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [Required(AllowEmptyStrings = false)]
    private string? proposedStudyPlanTemplateFilePath;

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
        ToastNotificationService toastNotificationService, FileDialogService fileDialogService)
    {
        this.userSettingsService = userSettingsService;
        this.toastNotificationService = toastNotificationService;
        this.fileDialogService = fileDialogService;

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

    partial void OnCreditTransferMemoTemplateFilePathChanged(string? value)
    {
        ValidateProperty(value, nameof(CreditTransferMemoTemplateFilePath));
    }

    partial void OnProposedStudyPlanTemplateFilePathChanged(string? value)
    {
        ValidateProperty(value, nameof(ProposedStudyPlanTemplateFilePath));
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
                 string.IsNullOrWhiteSpace(UserDepartment) || string.IsNullOrWhiteSpace(UserEmail) ||
                 string.IsNullOrWhiteSpace(CreditTransferMemoTemplateFilePath) ||
                 string.IsNullOrWhiteSpace(ProposedStudyPlanTemplateFilePath));
    }

    private bool CanOpenFilePicker()
    {
        return !IsSaving;
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
        userSettings.CreditTransferMemoTemplateFilePath = CreditTransferMemoTemplateFilePath;
        userSettings.ProposedStudyPlanTemplateFilePath = ProposedStudyPlanTemplateFilePath;
        userSettings.SearchDebounceSeconds = SearchDelay;
        userSettings.DataGridPageSize = DataGridPageSize;

        await userSettingsService.SetAllUserSettings(userSettings);

        toastNotificationService.ShowToastNotification(SETTINGS_SAVED_TITLE, SETTINGS_SAVED_BODY,
            NotificationType.Success);

        IsSaving = false;
    }

    [RelayCommand(CanExecute = nameof(CanOpenFilePicker))]
    private async Task SelectCreditTransferMemoTemplate()
    {
        await SelectTemplate(true);
    }

    [RelayCommand(CanExecute = nameof(CanOpenFilePicker))]
    private async Task SelectProposedStudyPlanTemplate()
    {
        await SelectTemplate(false);
    }
    #endregion

    #region Helpers
    private async Task SelectTemplate(bool isCreditTransferMemoTemplate)
    {
        var templateFiles = await fileDialogService.OpenFileDialog(SELECT_TEMPLATE_DIALOG_TITLE, false,
            FileDialogService.WordDocumentFilePickerFileType);
        if (templateFiles.Count == 0)
        {
            return;
        }

        var templateFilePath = templateFiles[0].Path.ToString();
        if (isCreditTransferMemoTemplate)
        {
            CreditTransferMemoTemplateFilePath = templateFilePath;
        }
        else
        {
            ProposedStudyPlanTemplateFilePath = templateFilePath;
        }
    }
    #endregion
}
