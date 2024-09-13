using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
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

    public StudyPlansPageViewModel(DatabaseService databaseService, UserSettingsService userSettingsService,
        GenericDialogService genericDialogService, ToastNotificationService toastNotificationService) : base(
        databaseService, userSettingsService, genericDialogService, toastNotificationService)
    {
    }
    #endregion

    #region BasePageView
    protected override string ObjectTypeName => "Study Plan";

    public override void UpdateItems()
    {
        Items.Clear();

        // Using the Include to eagerly load the student and university so they are ready on first page view
        Items.AddRange(DatabaseService.StudyPlans
            .Include(studyPlan => studyPlan.Student)
            .Include(studyPlan => studyPlan.DestinationUniversity));
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

    protected override string GetName(StudyPlan? item)
    {
        return item is not null
            ? $"{item.Student.Name} to {item.DestinationUniversity.Name} {ObjectTypeName}"
            : ObjectTypeName;
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

    protected override bool CanCreate()
    {
        if (Design.IsDesignMode)
        {
            return base.CanCreate();
        }

        return base.CanCreate() && DatabaseService.Universities.Count() >= 2 && DatabaseService.Students.Any();
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
