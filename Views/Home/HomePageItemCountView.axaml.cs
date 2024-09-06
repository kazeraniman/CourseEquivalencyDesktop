using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace CourseEquivalencyDesktop.Views.Home;

public class HomePageItemCountView : TemplatedControl
{
    #region Avalonia Properties
    public static readonly StyledProperty<Geometry?> IconProperty =
        Path.DataProperty.AddOwner<HomePageItemCountView>();

    public static readonly StyledProperty<object?> LabelProperty =
        ContentControl.ContentProperty.AddOwner<HomePageItemCountView>();

    public static readonly StyledProperty<string?> CountProperty =
        TextBlock.TextProperty.AddOwner<HomePageItemCountView>();
    #endregion

    #region Properties
    public Geometry? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public object? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string? Count
    {
        get => GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }
    #endregion

    #region Templated Control
    /// <summary>
    ///     Set the geometry on the path as it is not set by itself despite the binding due to how paths set their initial
    ///     geometry.
    ///     See <see cref="Path" /> for more details (and its inheritance from <see cref="Shape" />).
    /// </summary>
    /// <param name="e">Event args.</param>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var path = e.NameScope.Find<Path>("TextBoxPath");
        if (path != null)
        {
            path.Data = Icon;
        }
    }
    #endregion
}
