using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Styling;
using CourseEquivalencyDesktop.Views.StudyPlans;

namespace CourseEquivalencyDesktop.Utility.DataGridBoundColumns;

public class DataGridStudyPlanStatusColumn : DataGridBoundColumn
{
    #region Constants / Static Readonly
    private static readonly Thickness margin = new(5);
    #endregion

    protected override Control GenerateElement(DataGridCell cell, object dataItem)
    {
        var element = new StudyPlanStatusBadgeView
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = margin,
            Styles =
            {
                new Style(ele => ele.Is<StudyPlanStatusBadgeView>())
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
            element.Bind(StudyPlanStatusBadgeView.StudyPlanStatusProperty, Binding);
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
}
