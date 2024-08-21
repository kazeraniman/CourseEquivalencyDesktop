using CourseEquivalencyDesktop.ViewModels;

namespace CourseEquivalencyDesktop.Utility;

public class NavigationPageInfo(string name, string icon, ViewModelBase viewModel)
{
    public string Name { get; init; } = name;
    public string Icon { get; init; } = icon;
    public ViewModelBase ViewModel { get; init; } = viewModel;
}

