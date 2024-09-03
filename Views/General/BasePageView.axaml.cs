using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.Views.CustomControls;

namespace CourseEquivalencyDesktop.Views.General;

public class BasePageView : TemplatedControl
{
    #region Constants
    private const double DEFAULT_SPACING = 30;
    private const double NO_HEADER_SPACING = 0;
    #endregion

    #region Fields
    private DataGrid? dataGrid;
    #endregion

    #region Avalonia Properties
    public static readonly StyledProperty<string?> HeaderProperty =
        AvaloniaProperty.Register<BasePageView, string?>(nameof(HeaderProperty));

    public static readonly StyledProperty<int> DefaultSortColumnIndexProperty =
        AvaloniaProperty.Register<BasePageView, int>(nameof(DefaultSortColumnIndexProperty));

    public static readonly StyledProperty<string?> SearchTextProperty =
        AvaloniaProperty.Register<BasePageView, string?>(nameof(SearchTextProperty),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<ICommand?> CreateCommandProperty =
        AvaloniaProperty.Register<BasePageView, ICommand?>(nameof(CreateCommandProperty), enableDataValidation: true);

    public static readonly StyledProperty<ICommand?> PreviousPageCommandProperty =
        AvaloniaProperty.Register<BasePageView, ICommand?>(nameof(PreviousPageCommandProperty),
            enableDataValidation: true);

    public static readonly StyledProperty<ICommand?> NextPageCommandProperty =
        AvaloniaProperty.Register<BasePageView, ICommand?>(nameof(NextPageCommandProperty), enableDataValidation: true);

    public static readonly StyledProperty<DataGridCollectionView> ItemsSourceProperty =
        AvaloniaProperty.Register<BasePageView, DataGridCollectionView>(nameof(ItemsSourceProperty),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<ObservableCollection<DataGridColumn>> DataGridColumnsProperty =
        AvaloniaProperty.Register<BasePageView, ObservableCollection<DataGridColumn>>(nameof(DataGridColumnsProperty),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string?> CurrentHumanReadablePageIndexProperty =
        AvaloniaProperty.Register<BasePageView, string?>(nameof(CurrentHumanReadablePageIndexProperty),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<bool> ShouldShowHeaderProperty =
        AvaloniaProperty.Register<BasePageView, bool>(nameof(ShouldShowHeaderProperty), true);

    public static readonly StyledProperty<bool> ShouldShowCreateButtonProperty =
        AvaloniaProperty.Register<BasePageView, bool>(nameof(ShouldShowCreateButtonProperty), true);
    #endregion

    #region Properties
    public string? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public int DefaultSortColumnIndex
    {
        get => GetValue(DefaultSortColumnIndexProperty);
        set => SetValue(DefaultSortColumnIndexProperty, value);
    }

    public string? SearchText
    {
        get => GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    public ICommand? CreateCommand
    {
        get => GetValue(CreateCommandProperty);
        set => SetValue(CreateCommandProperty, value);
    }

    public ICommand? PreviousPageCommand
    {
        get => GetValue(PreviousPageCommandProperty);
        set => SetValue(PreviousPageCommandProperty, value);
    }

    public ICommand? NextPageCommand
    {
        get => GetValue(NextPageCommandProperty);
        set => SetValue(NextPageCommandProperty, value);
    }

    public DataGridCollectionView ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public ObservableCollection<DataGridColumn> DataGridColumns
    {
        get => GetValue(DataGridColumnsProperty);
        set => SetValue(DataGridColumnsProperty, value);
    }

    public string? CurrentHumanReadablePageIndex
    {
        get => GetValue(CurrentHumanReadablePageIndexProperty);
        set => SetValue(CurrentHumanReadablePageIndexProperty, value);
    }

    public bool ShouldShowHeader
    {
        get => GetValue(ShouldShowHeaderProperty);
        set => SetValue(ShouldShowHeaderProperty, value);
    }

    public bool ShouldShowCreateButton
    {
        get => GetValue(ShouldShowCreateButtonProperty);
        set => SetValue(ShouldShowCreateButtonProperty, value);
    }
    #endregion

    #region Constructors
    public BasePageView()
    {
        DataGridColumns = [];
    }
    #endregion

    #region TemplatedControl
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        dataGrid = e.NameScope.Find<DataGrid>("DataGrid");
        if (dataGrid is null)
        {
            return;
        }

        dataGrid.Columns.Clear();
        dataGrid.Columns.AddRange(DataGridColumns);

        var pageHeader = e.NameScope.Find<TextBlock>("PageHeader");
        var pageRoot = e.NameScope.Find<StackPanel>("PageRoot");
        if (pageRoot is not null && pageHeader is not null)
        {
            pageHeader.IsVisible = ShouldShowHeader;
            pageRoot.Spacing = ShouldShowHeader ? DEFAULT_SPACING : NO_HEADER_SPACING;
        }

        var createButton = e.NameScope.Find<Button>("CreateButton");
        if (createButton is not null)
        {
            createButton.IsVisible = ShouldShowCreateButton;
        }
    }
    #endregion

    #region Public Functionality
    public void ApplyInitialSort()
    {
        dataGrid?.Columns[DefaultSortColumnIndex].Sort(ListSortDirection.Ascending);
    }
    #endregion
}
