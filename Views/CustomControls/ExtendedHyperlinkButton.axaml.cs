using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;

namespace CourseEquivalencyDesktop.Views.CustomControls;

public class ExtendedHyperlinkButton : HyperlinkButton
{
    #region Fields
    private Uri? emailUri;
    #endregion

    #region Avalonia Properties
    public static readonly StyledProperty<bool> IsEmailProperty =
        AvaloniaProperty.Register<ExtendedHyperlinkButton, bool>(nameof(IsEmail));
    #endregion

    #region Properties
    public bool IsEmail
    {
        get => GetValue(IsEmailProperty);
        set => SetValue(IsEmailProperty, value);
    }
    #endregion

    #region HyperlinkButton
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty)
        {
            SetVisibility();
        }

        if (!IsEmail)
        {
            return;
        }

        if (change.Property == IsEmailProperty || change.Property == NavigateUriProperty)
        {
            SetEmailUri();
        }

        void SetEmailUri()
        {
            if (NavigateUri is null)
            {
                return;
            }

            emailUri = new Uri($"mailto:{NavigateUri}");
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        SetVisibility();
    }

    protected override void OnClick()
    {
        var uri = IsEmail ? emailUri : NavigateUri;
        if (uri is null)
        {
            return;
        }

        Dispatcher.UIThread.Post(OpenUrl);

        async void OpenUrl()
        {
            var success = await TopLevel.GetTopLevel(this)!.Launcher.LaunchUriAsync(uri);
            if (success)
            {
                SetCurrentValue(IsVisitedProperty, true);
            }
        }
    }
    #endregion

    #region Helpers
    private void SetVisibility()
    {
        IsVisible = Content is not null;
    }
    #endregion
}
