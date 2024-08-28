using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace CourseEquivalencyDesktop.Views.CustomControls;

public class FormInputComboBox : ComboBox
{
    #region Avalonia Properties
    public static readonly StyledProperty<object?> LabelTextProperty =
        ContentControl.ContentProperty.AddOwner<FormInputTextBox>();

    public static readonly StyledProperty<Geometry?> IconProperty =
        Path.DataProperty.AddOwner<FormInputTextBox>();
    #endregion

    #region Properties
    public object? LabelText
    {
        get => GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public Geometry? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
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

        Classes.Add("StyledTextBox");

        var path = e.NameScope.Find<Path>("ComboBoxPath");
        if (path != null)
        {
            path.Data = Icon;
        }
    }
    #endregion
}
