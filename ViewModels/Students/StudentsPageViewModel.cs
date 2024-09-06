using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;
using Microsoft.EntityFrameworkCore;

namespace CourseEquivalencyDesktop.ViewModels.Students;

public class StudentsPageViewModel : BasePageViewModel<Student>
{
    #region Constants
    private const string STUDENT_DELETE_BODY =
        "Are you sure you wish to delete \"{0}\"?\nThis action cannot be undone and will delete all associated entries.";
    #endregion

    #region Constructors
    public StudentsPageViewModel()
    {
    }

    public StudentsPageViewModel(DatabaseService databaseService, UserSettingsService userSettingsService,
        GenericDialogService genericDialogService) : base(databaseService, userSettingsService, genericDialogService)
    {
    }
    #endregion

    #region BasePageView
    protected override string DeleteTitle => "Delete Student?";
    protected override string DeleteFailedTitle => "Student Deletion Failed";
    protected override string DeleteFailedBody => "An error occurred and the student could not be deleted.";

    public override void UpdateItems()
    {
        Items.Clear();

        // Using the Include to eagerly load the universities so they are ready on first page view
        Items.AddRange(DatabaseService.Students.Include(student => student.University));
    }

    protected override Task<HashSet<Student>> Remove(Student item)
    {
        DatabaseService.Students.Remove(item);
        return Task.FromResult<HashSet<Student>>([item]);
    }

    protected override string GetDeleteBody(Student item)
    {
        return string.Format(STUDENT_DELETE_BODY, item.Name);
    }

    protected override bool Filter(object arg)
    {
        if (arg is not Student student)
        {
            return false;
        }

        return string.IsNullOrWhiteSpace(SearchText) || student.Name.CaseInsensitiveContains(SearchText) ||
               student.University.Name.CaseInsensitiveContains(SearchText) ||
               student.StudentId.CaseInsensitiveContains(SearchText);
    }

    protected override bool CanCreate()
    {
        if (Design.IsDesignMode)
        {
            return base.CanCreate();
        }

        return base.CanCreate() && DatabaseService.Universities.Any();
    }
    #endregion
}
