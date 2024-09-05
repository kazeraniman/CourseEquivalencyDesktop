using System.Collections.Generic;
using System.Threading.Tasks;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;
using Microsoft.EntityFrameworkCore;

namespace CourseEquivalencyDesktop.ViewModels.StudyPlans;

public class StudyPlansPageViewModel : BasePageViewModel<StudyPlan>
{
    #region Constants
    private const string STUDY_PLAN_DELETE_BODY =
        "Are you sure you wish to delete the study plan for \"{0}\" to \"{1}\"?\nThis action cannot be undone and will delete all associated entries.";
    #endregion

    #region Constructors
    public StudyPlansPageViewModel()
    {
    }

    public StudyPlansPageViewModel(DatabaseService databaseService,
        UserSettingsService userSettingsService,
        GenericDialogService genericDialogService) : base(databaseService, userSettingsService, genericDialogService)
    {
    }
    #endregion

    #region BasePageView
    protected override string DeleteTitle => "Delete Study Plan?";
    protected override string DeleteFailedTitle => "Study Plan Deletion Failed";
    protected override string DeleteFailedBody => "An error occurred and the study plan could not be deleted.";

    public override void UpdateItems()
    {
        Items.Clear();

        // Using the Include to eagerly load equivalencies and their universities so they are ready on first page view
        Items.AddRange(DatabaseService.StudyPlans
            .Include(studyPlan => studyPlan.Student)
            .Include(studyPlan => studyPlan.DestinationUniversity));
        // TODO: Possibly need to include courses, and then the universities and equivalencies on them too for the edit window, but maybe i just do that in there since it can come from create too
    }

    protected override Task<HashSet<StudyPlan>> Remove(StudyPlan item)
    {
        DatabaseService.StudyPlans.Remove(item);
        return Task.FromResult<HashSet<StudyPlan>>([item]);
    }

    protected override string GetDeleteBody(StudyPlan item)
    {
        return string.Format(STUDY_PLAN_DELETE_BODY, item.Student.Name, item.DestinationUniversity.Name);
    }

    protected override bool Filter(object arg)
    {
        if (arg is not StudyPlan studyPlan)
        {
            return false;
        }

        return string.IsNullOrWhiteSpace(SearchText) ||
               studyPlan.Student.StudentId.CaseInsensitiveContains(SearchText) ||
               studyPlan.Student.Name.CaseInsensitiveContains(SearchText) ||
               studyPlan.DestinationUniversity.Name.CaseInsensitiveContains(SearchText);
    }

    protected override async Task<StudyPlan?> CreateInternal()
    {
        var createdStudyPlan = await base.CreateInternal();
        if (createdStudyPlan is null)
        {
            return null;
        }

        return await Edit(createdStudyPlan);
    }
    #endregion
}
