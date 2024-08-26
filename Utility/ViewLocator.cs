using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CourseEquivalencyDesktop.ViewModels;
using CourseEquivalencyDesktop.ViewModels.General;

namespace CourseEquivalencyDesktop.Utility;

/// <summary>
/// Manages loading views by name. Useful in conjunction with <see cref="TransitioningContentControl"/>.
/// </summary>
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
        {
            return null;
        }

        var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type == null)
        {
            return new TextBlock { Text = "Not Found: " + name };
        }

        var control = (Control)Activator.CreateInstance(type)!;
        control.DataContext = data;
        return control;
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}