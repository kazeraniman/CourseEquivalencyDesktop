using System.Collections.Generic;
using System.Threading.Tasks;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;
using Microsoft.EntityFrameworkCore;

namespace CourseEquivalencyDesktop.ViewModels.Equivalencies;

public class EquivalenciesPageViewModel : BasePageViewModel<CourseEquivalency>
{
    #region Constants
    private const string EQUIVALENCY = "Equivalency";

    private const string EQUIVALENCY_DELETE_BODY =
        "Are you sure you wish to delete the equivalency between \"{0}\" and \"{1}\"?\nThis action cannot be undone and will delete all associated entries.";
    #endregion

    #region Constructors
    public EquivalenciesPageViewModel()
    {
    }

    public EquivalenciesPageViewModel(DatabaseService databaseService, UserSettingsService userSettingsService,
        GenericDialogService genericDialogService, ToastNotificationService toastNotificationService) : base(
        databaseService, userSettingsService, genericDialogService, toastNotificationService)
    {
    }
    #endregion

    #region BasePageView
    protected override string ObjectTypeName => EQUIVALENCY;

    public override void UpdateItems()
    {
        Items.Clear();

        // Using the Include to eagerly load equivalencies and their universities so they are ready on first page view
        Items.AddRange(DatabaseService.Equivalencies
            .Include(equivalency => equivalency.Course)
            .ThenInclude(course => course.University)
            .Include(equivalency => equivalency.EquivalentCourse)
            .ThenInclude(equivalentCourse => equivalentCourse.University));
    }

    protected override async Task<HashSet<CourseEquivalency>> Remove(CourseEquivalency item)
    {
        item.Course.Equivalencies.Remove(item.EquivalentCourse);
        item.EquivalentCourse.Equivalencies.Remove(item.Course);

        var reverseEquivalency =
            await DatabaseService.Equivalencies.FindAsync(item.EquivalentCourse.Id, item.Course.Id);
        var removedItems = new HashSet<CourseEquivalency> { item };
        if (reverseEquivalency is not null)
        {
            removedItems.Add(reverseEquivalency);
        }

        return removedItems;
    }

    protected override string GetDeleteBody(CourseEquivalency item)
    {
        return string.Format(EQUIVALENCY_DELETE_BODY, item.Course.Name, item.EquivalentCourse.Name);
    }

    protected override string GetName(CourseEquivalency? item)
    {
        return GetEquivalencyName(item);
    }

    protected override bool Filter(object arg)
    {
        if (arg is not CourseEquivalency courseEquivalency)
        {
            return false;
        }

        return string.IsNullOrWhiteSpace(SearchText) ||
               courseEquivalency.Course.Name.CaseInsensitiveContains(SearchText) ||
               courseEquivalency.Course.University.Name.CaseInsensitiveContains(SearchText) ||
               courseEquivalency.Course.CourseId.CaseInsensitiveContains(SearchText) ||
               courseEquivalency.EquivalentCourse.Name.CaseInsensitiveContains(SearchText) ||
               courseEquivalency.EquivalentCourse.University.Name.CaseInsensitiveContains(SearchText) ||
               courseEquivalency.EquivalentCourse.CourseId.CaseInsensitiveContains(SearchText);
    }
    #endregion

    #region Helpers
    public static string GetEquivalencyName(CourseEquivalency? item)
    {
        return item is not null ? GetEquivalencyName(item.Course, item.EquivalentCourse) : EQUIVALENCY;
    }

    public static string GetEquivalencyName(Course item, Course equivalentCourse)
    {
        return $"{item.Name} to {equivalentCourse.Name} {EQUIVALENCY}";
    }
    #endregion
}
