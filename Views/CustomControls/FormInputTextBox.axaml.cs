using Avalonia;
using Avalonia.Controls;

namespace CourseEquivalencyDesktop.Views.CustomControls;

/// <remarks>
///     Based on my understanding of templated controls, I could either inherit and lessen the code duplication but
///     increase the axaml duplication, or I could not inherit and pass everything through by having a ton of code
///     duplication but lessen the axaml duplication. I've gone for less code for now.
/// </remarks>
public class FormInputTextBox : InputTextBox
{
    #region Avalonia Properties
    public static readonly StyledProperty<object?> LabelTextProperty =
        ContentControl.ContentProperty.AddOwner<FormInputTextBox>();
    #endregion

    #region Properties
    public object? LabelText
    {
        get => GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }
    #endregion
}
