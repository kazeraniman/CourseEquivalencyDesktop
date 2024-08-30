using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Styling;

namespace CourseEquivalencyDesktop.Utility.DataGridBoundColumns;

public class DataGridSelectableTextColumn : DataGridBoundColumn
{
    #region Constants / Static Readonly
    private const int MAX_HEIGHT = 30;

    private static readonly Thickness padding = new(10, 5);
    #endregion

    #region DataGridBoundColumn
    protected override Control GenerateElement(DataGridCell cell, object dataItem)
    {
        var element = new SelectableTextBlock
        {
            VerticalAlignment = VerticalAlignment.Center,
            Padding = padding,
            MaxHeight = MAX_HEIGHT,
            Styles =
            {
                new Style(ele => ele.Is<SelectableTextBlock>())
                {
                    Setters =
                    {
                        new Setter(ToolTip.TipProperty, Binding)
                    }
                }
            }
        };

        if (Binding != null)
        {
            element.Bind(TextBlock.TextProperty, Binding);
        }

        return element;
    }

    protected override object PrepareCellForEdit(Control editingElement, RoutedEventArgs editingEventArgs)
    {
        throw new NotImplementedException();
    }

    protected override Control GenerateEditingElementDirect(DataGridCell cell, object dataItem)
    {
        throw new NotImplementedException();
    }
    #endregion
}
