using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Media;
using CourseEquivalencyDesktop.Models;

namespace CourseEquivalencyDesktop.Views.StudyPlans;

public class StudyPlanStatusBadgeView : TemplatedControl
{
    #region Fields
    private Border? border;
    private TextBlock? textBlock;
    private Path? path;
    #endregion

    #region Avalonia Properties
    public static readonly StyledProperty<StudyPlanStatus> StudyPlanStatusProperty =
        AvaloniaProperty.Register<StudyPlanStatusBadgeView, StudyPlanStatus>(nameof(StudyPlanStatus),
            defaultBindingMode: BindingMode.TwoWay);
    #endregion

    #region Properties
    public StudyPlanStatus StudyPlanStatus
    {
        get => GetValue(StudyPlanStatusProperty);
        set => SetValue(StudyPlanStatusProperty, value);
    }
    #endregion

    #region TemplatedControl
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property != StudyPlanStatusProperty)
        {
            return;
        }

        UpdateVisuals();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        border = e.NameScope.Find<Border>("Border");
        textBlock = e.NameScope.Find<TextBlock>("TextBlock");
        path = e.NameScope.Find<Path>("Path");

        UpdateVisuals();
    }
    #endregion

    #region Helpers
    private void UpdateVisuals()
    {
        var (pathIcon, backgroundColour) = GetPathResources();

        if (border is not null)
        {
            border.Background = backgroundColour;
        }

        if (textBlock is not null)
        {
            textBlock.Text = StudyPlanStatus.ToString();
        }

        if (path is not null)
        {
            path.Data = pathIcon;
        }
    }

    private (StreamGeometry?, SolidColorBrush?) GetPathResources()
    {
        var (pathResourceName, backgroundColourResourceName) = StudyPlanStatus switch
        {
            StudyPlanStatus.Pending => ("PendingIconData", "BackgroundGrey100"),
            StudyPlanStatus.Cancelled => ("CancelledIconData", "BackgroundRed300"),
            StudyPlanStatus.Complete => ("CompleteIconData", "BackgroundGreen300"),
            _ => throw new ArgumentOutOfRangeException()
        };

        if (Application.Current == null)
        {
            return (null, null);
        }

        Application.Current.TryFindResource(pathResourceName, out var path);
        Application.Current.TryFindResource(backgroundColourResourceName, out var backgroundColour);

        SolidColorBrush? solidColorBrush = null;
        if (backgroundColour is Color colour)
        {
            solidColorBrush = new SolidColorBrush(colour);
        }

        return (path as StreamGeometry, solidColorBrush);
    }
    #endregion
}
