using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace CourseEquivalencyDesktop.Views.General;

public class BaseCreateOrEditWindow : TemplatedControl
{
    #region Fields
    private WindowNotificationManager? windowNotificationManager;
    #endregion

    #region Avalonia Properties
    public static readonly StyledProperty<string?> WindowAndButtonTextProperty =
        AvaloniaProperty.Register<BasePageView, string?>(nameof(WindowAndButtonTextProperty));

    public static readonly StyledProperty<ICommand?> CancelCommandProperty =
        AvaloniaProperty.Register<BasePageView, ICommand?>(nameof(CancelCommandProperty),
            enableDataValidation: true);

    public static readonly StyledProperty<ICommand?> CreateOrEditCommandProperty =
        AvaloniaProperty.Register<BasePageView, ICommand?>(nameof(CreateOrEditCommandProperty),
            enableDataValidation: true);

    public static readonly StyledProperty<IList<Control>> FormContentsProperty =
        AvaloniaProperty.Register<BasePageView, IList<Control>>(nameof(FormContentsProperty),
            enableDataValidation: true, defaultBindingMode: BindingMode.TwoWay);
    #endregion

    #region Properties
    public string? WindowAndButtonText
    {
        get => GetValue(WindowAndButtonTextProperty);
        set => SetValue(WindowAndButtonTextProperty, value);
    }

    public ICommand? CancelCommand
    {
        get => GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }

    public ICommand? CreateOrEditCommand
    {
        get => GetValue(CreateOrEditCommandProperty);
        set => SetValue(CreateOrEditCommandProperty, value);
    }

    public IList<Control> FormContents
    {
        get => GetValue(FormContentsProperty);
        set => SetValue(FormContentsProperty, value);
    }
    #endregion

    #region Constructors
    public BaseCreateOrEditWindow()
    {
        FormContents = [];
    }
    #endregion

    #region TemplatedControl
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        windowNotificationManager =
            e.NameScope.Find<WindowNotificationManager>("CreateOrEditWindowNotificationManager");

        var stackPanel = e.NameScope.Find<StackPanel>("ContentsStackPanel");
        if (stackPanel is null)
        {
            return;
        }

        stackPanel.Children.Clear();
        stackPanel.Children.AddRange(FormContents);
    }
    #endregion

    #region Additional Functionality
    public WindowNotificationManager? GetWindowNotificationManager()
    {
        return windowNotificationManager;
    }
    #endregion
}
