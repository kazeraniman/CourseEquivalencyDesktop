using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CourseEquivalencyDesktop.ViewModels.General;

public partial class GenericDialogWindowViewModel : ViewModelBase
{
    public class GenericDialogEventArgs : EventArgs
    {
        public bool WasPrimary { get; init; }
    }

    #region Properties
    #region Observable Properties
    [ObservableProperty]
    private string titleText;

    [ObservableProperty]
    private string bodyText;

    [ObservableProperty]
    private string primaryButtonText;

    [ObservableProperty]
    private string? secondaryButtonText;
    #endregion
    #endregion

    #region Events
    public event EventHandler? OnRequestCloseWindow;
    #endregion

    #region Constructors
    public GenericDialogWindowViewModel()
    {
        Utility.Utility.AssertDesignMode();

        titleText = "Generic Dialog Window";
        bodyText = "Generic dialog window body text. Please ignore.";
        primaryButtonText = "Ok";
    }

    public GenericDialogWindowViewModel(string titleText, string bodyText, string primaryButtonText,
        string? secondaryButtonText)
    {
        this.titleText = titleText;
        this.bodyText = bodyText;
        this.primaryButtonText = primaryButtonText;
        this.secondaryButtonText = secondaryButtonText;
    }
    #endregion

    #region Commands
    [RelayCommand]
    private void Primary()
    {
        OnRequestCloseWindow?.Invoke(this, new GenericDialogEventArgs { WasPrimary = true });
    }

    [RelayCommand]
    private void Secondary()
    {
        OnRequestCloseWindow?.Invoke(this, new GenericDialogEventArgs { WasPrimary = false });
    }
    #endregion
}
