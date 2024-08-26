using CourseEquivalencyDesktop.ViewModels.General;

namespace CourseEquivalencyDesktop.Utility;

/// <summary>
///     Represents a page to which you can navigate with the sidebar.
/// </summary>
/// <param name="name">The page's display name.</param>
/// <param name="icon">The icon to use for navigation.</param>
/// <param name="viewModel">The ViewModel for the page.</param>
public class NavigationPageInfo(string name, string icon, ViewModelBase viewModel)
{
    public string Name { get; init; } = name;
    public string Icon { get; init; } = icon;
    public ViewModelBase ViewModel { get; init; } = viewModel;
}
