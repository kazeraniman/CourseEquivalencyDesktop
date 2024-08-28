using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Media;

namespace CourseEquivalencyDesktop.Views.CustomControls;

public class FormInputTextBox : ContentControl
{
    #region Avalonia Properties
    public static readonly StyledProperty<string> LabelTextProperty =
        AvaloniaProperty.Register<FormInputTextBox, string>(nameof(LabelText));

    public static readonly StyledProperty<string> TextBoxTextProperty =
        AvaloniaProperty.Register<FormInputTextBox, string>(nameof(TextBoxText),
            defaultBindingMode: BindingMode.TwoWay, enableDataValidation: true);

    public static readonly StyledProperty<string> TextBoxWatermarkProperty =
        AvaloniaProperty.Register<FormInputTextBox, string>(nameof(TextBoxWatermark));

    public static readonly StyledProperty<StreamGeometry> TextBoxIconProperty =
        AvaloniaProperty.Register<FormInputTextBox, StreamGeometry>(nameof(TextBoxIcon),
            defaultBindingMode: BindingMode.TwoWay);
    #endregion

    #region Properties
    public string LabelText
    {
        get => GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public string TextBoxText
    {
        get => GetValue(TextBoxTextProperty);
        set => SetValue(TextBoxTextProperty, value);
    }

    public string TextBoxWatermark
    {
        get => GetValue(TextBoxWatermarkProperty);
        set => SetValue(TextBoxWatermarkProperty, value);
    }

    public StreamGeometry TextBoxIcon
    {
        get => GetValue(TextBoxIconProperty);
        set => SetValue(TextBoxIconProperty, value);
    }
    #endregion

    #region Templated Control
    /// <summary>
    ///     Set the geometry on the path as it is not set by itself despite the binding.
    ///     TODO: Investigate why
    /// </summary>
    /// <param name="e">Event args.</param>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var path = e.NameScope.Find<Path>("TextBoxPath");
        if (path != null)
        {
            path.Data = TextBoxIcon;
        }
    }

    /// <summary>
    ///     Forward validation errors from the text component.
    /// </summary>
    /// <param name="property">The property with validation errors</param>
    /// <param name="state">Information about the binding value.</param>
    /// <param name="error">The error in question, if any.</param>
    protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
    {
        if (property == TextBoxTextProperty)
        {
            DataValidationErrors.SetError(this, error);
        }
    }
    #endregion
}
