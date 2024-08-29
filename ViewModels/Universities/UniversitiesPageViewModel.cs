using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;

namespace CourseEquivalencyDesktop.ViewModels.Universities;

public class UniversitiesPageViewModel : BasePageViewModel<University>
{
    #region Constants
    private const string UNIVERSITY_DELETE_BODY =
        "Are you sure you wish to delete \"{0}\"?\nThis action cannot be undone and will delete all associated entries.";
    #endregion

    #region Constructors
    public UniversitiesPageViewModel()
    {
    }

    public UniversitiesPageViewModel(DatabaseService databaseService, UserSettingsService userSettingsService,
        GenericDialogService genericDialogService) : base(databaseService, userSettingsService, genericDialogService)
    {
    }
    #endregion

    #region BasePageView
    protected override string DeleteTitle => "Delete University?";
    protected override string DeleteFailedTitle => "University Deletion Failed";
    protected override string DeleteFailedBody => "An error occurred and the university could not be deleted.";

    public override void UpdateItems()
    {
        Items.Clear();
        Items.AddRange(DatabaseService.Universities);
    }

    protected override void Remove(University item)
    {
        DatabaseService.Universities.Remove(item);
    }

    protected override string GetDeleteBody(University item)
    {
        return string.Format(UNIVERSITY_DELETE_BODY, item.Name);
    }

    protected override bool Filter(object arg)
    {
        if (arg is not University university)
        {
            return false;
        }

        return string.IsNullOrWhiteSpace(SearchText) || university.Name.CaseInsensitiveContains(SearchText);
    }
    #endregion
}
