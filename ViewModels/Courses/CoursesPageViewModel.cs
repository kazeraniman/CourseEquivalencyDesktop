using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;

namespace CourseEquivalencyDesktop.ViewModels.Courses;

public class CoursesPageViewModel : BasePageViewModel<Course>
{
    #region Constants
    private const string COURSE_DELETE_BODY =
        "Are you sure you wish to delete \"{0}\"?\nThis action cannot be undone and will delete all associated entries.";
    #endregion

    #region Constructors
    public CoursesPageViewModel()
    {
    }

    public CoursesPageViewModel(DatabaseService databaseService, UserSettingsService userSettingsService,
        GenericDialogService genericDialogService) : base(databaseService, userSettingsService, genericDialogService)
    {
    }
    #endregion

    #region BasePageView
    protected override string DeleteTitle => "Delete Course?";
    protected override string DeleteFailedTitle => "Course Deletion Failed";
    protected override string DeleteFailedBody => "An error occurred and the course could not be deleted.";

    public override void UpdateItems()
    {
        Items.Clear();
        Items.AddRange(DatabaseService.Courses);
    }

    protected override void Remove(Course item)
    {
        DatabaseService.Courses.Remove(item);
    }

    protected override string GetDeleteBody(Course item)
    {
        return string.Format(COURSE_DELETE_BODY, item.Name);
    }

    protected override bool Filter(object arg)
    {
        if (arg is not Course course)
        {
            return false;
        }

        return string.IsNullOrWhiteSpace(SearchText) || course.Name.CaseInsensitiveContains(SearchText) ||
               course.University.Name.CaseInsensitiveContains(SearchText) ||
               course.CourseId.CaseInsensitiveContains(SearchText);
    }
    #endregion
}
