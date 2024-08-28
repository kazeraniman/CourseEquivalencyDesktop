using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;

namespace CourseEquivalencyDesktop.Views.CustomControls;

public class FormInputTextBox : ContentControl
{
    #region Fields
    private TextBox? mainTextBox;
    #endregion

    #region Avalonia Properties
    public static readonly StyledProperty<object?> LabelTextProperty =
        ContentProperty.AddOwner<FormInputTextBox>();

    public static readonly StyledProperty<string?> TextProperty =
        TextBox.TextProperty.AddOwner<FormInputTextBox>(
            new StyledPropertyMetadata<string?>(defaultBindingMode: BindingMode.TwoWay, enableDataValidation: true));

    public static readonly StyledProperty<string?> WatermarkProperty =
        TextBox.WatermarkProperty.AddOwner<FormInputTextBox>();

    public static readonly StyledProperty<Geometry?> IconProperty =
        Path.DataProperty.AddOwner<FormInputTextBox>();

    public static readonly StyledProperty<int> MinLinesProperty =
        TextBox.MinLinesProperty.AddOwner<FormInputTextBox>(new StyledPropertyMetadata<int>(1));

    public static readonly StyledProperty<int> MaxLinesProperty =
        TextBox.MaxLinesProperty.AddOwner<FormInputTextBox>(new StyledPropertyMetadata<int>(1));

    public static readonly StyledProperty<TextWrapping> TextWrappingProperty =
        TextBox.TextWrappingProperty.AddOwner<FormInputTextBox>(
            new StyledPropertyMetadata<TextWrapping>(TextWrapping.NoWrap));

    public static readonly StyledProperty<bool> AcceptsReturnProperty =
        TextBox.AcceptsReturnProperty.AddOwner<FormInputTextBox>(
            new StyledPropertyMetadata<bool>(false));

    public static readonly StyledProperty<HorizontalAlignment> TextHorizontalContentAlignmentProperty =
        TextBox.HorizontalContentAlignmentProperty.AddOwner<FormInputTextBox>(
            new StyledPropertyMetadata<HorizontalAlignment>(HorizontalAlignment.Stretch));

    public static readonly StyledProperty<VerticalAlignment> TextVerticalContentAlignmentProperty =
        TextBox.VerticalContentAlignmentProperty.AddOwner<FormInputTextBox>(
            new StyledPropertyMetadata<VerticalAlignment>(VerticalAlignment.Center));
    #endregion

    #region Properties
    public object? LabelText
    {
        get => GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string? Watermark
    {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    public Geometry? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public int MinLines
    {
        get => GetValue(MinLinesProperty);
        set => SetValue(MinLinesProperty, value);
    }

    public int MaxLines
    {
        get => GetValue(MaxLinesProperty);
        set => SetValue(MaxLinesProperty, value);
    }

    public TextWrapping TextWrapping
    {
        get => GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }

    public bool AcceptsReturn
    {
        get => GetValue(AcceptsReturnProperty);
        set => SetValue(AcceptsReturnProperty, value);
    }

    public HorizontalAlignment TextHorizontalContentAlignment
    {
        get => GetValue(TextHorizontalContentAlignmentProperty);
        set => SetValue(TextHorizontalContentAlignmentProperty, value);
    }

    public VerticalAlignment TextVerticalContentAlignment
    {
        get => GetValue(TextVerticalContentAlignmentProperty);
        set => SetValue(TextVerticalContentAlignmentProperty, value);
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

        mainTextBox = e.NameScope.Find<TextBox>("MainTextBox");
    }

    /// <summary>
    ///     Forward validation errors from the text component.
    /// </summary>
    /// <param name="property">The property with validation errors</param>
    /// <param name="state">Information about the binding value.</param>
    /// <param name="error">The error in question, if any.</param>
    protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
    {
        if (property == TextProperty)
        {
            DataValidationErrors.SetError(this, error);
        }
    }
    #endregion

    #region Additional Functionality
    public void Focus()
    {
        mainTextBox?.Focus();
        mainTextBox?.SelectAll();
    }
    #endregion
}
