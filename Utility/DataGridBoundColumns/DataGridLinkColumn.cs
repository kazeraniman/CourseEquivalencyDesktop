using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Styling;

namespace CourseEquivalencyDesktop.Utility.DataGridBoundColumns;

public class DataGridLinkColumn : DataGridBoundColumn
{
    #region Constants / Static Readonly
    private static readonly Thickness padding = new(10, 5);
    #endregion

    #region DataGridBoundColumn
    protected override Control GenerateElement(DataGridCell cell, object dataItem)
    {
        var element = new HyperlinkButton
        {
            VerticalAlignment = VerticalAlignment.Center,
            Padding = padding,
            Styles =
            {
                new Style(ele => ele.Is<HyperlinkButton>())
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
            element.Bind(ContentControl.ContentProperty, Binding);
            element.Bind(HyperlinkButton.NavigateUriProperty, Binding);
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
