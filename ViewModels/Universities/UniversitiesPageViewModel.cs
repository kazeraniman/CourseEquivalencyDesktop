using System.Collections.Generic;
using System.Threading.Tasks;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;

namespace CourseEquivalencyDesktop.ViewModels.Universities;

public class UniversitiesPageViewModel : BasePageViewModel<University>
{
    #region Constants
    private const string UNIVERSITY = "University";

    private const string UNIVERSITY_DELETE_BODY =
        "Are you sure you wish to delete \"{0}\"?\nThis action cannot be undone and will delete all associated entries.";
    #endregion

    #region Constructors
    public UniversitiesPageViewModel()
    {
    }

    public UniversitiesPageViewModel(DatabaseService databaseService, UserSettingsService userSettingsService,
        GenericDialogService genericDialogService, ToastNotificationService toastNotificationService) : base(
        databaseService, userSettingsService, genericDialogService, toastNotificationService)
    {
    }
    #endregion

    #region BasePageView
    protected override string DeleteTitle => "Delete University?";

    public override void UpdateItems()
    {
        Items.Clear();
        Items.AddRange(DatabaseService.Universities);
    }

    protected override Task<HashSet<University>> Remove(University item)
    {
        DatabaseService.Universities.Remove(item);
        return Task.FromResult<HashSet<University>>([item]);
    }

    protected override string GetDeleteBody(University item)
    {
        return string.Format(UNIVERSITY_DELETE_BODY, item.Name);
    }

    protected override string GetName(University? item)
    {
        return item?.Name ?? UNIVERSITY;
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
