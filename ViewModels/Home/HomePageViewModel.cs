using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.General;
using Microsoft.EntityFrameworkCore;

namespace CourseEquivalencyDesktop.ViewModels.Home;

public partial class HomePageViewModel : BaseViewModel
{
    #region Fields
    private readonly DatabaseService databaseService;
    #endregion

    #region Properties
    #region Observable Properties
    [ObservableProperty]
    private int universityCount;

    [ObservableProperty]
    private int courseCount;

    [ObservableProperty]
    private int equivalencyCount;

    [ObservableProperty]
    private int studentCount;

    [ObservableProperty]
    private int studyPlanCount;

    [ObservableProperty]
    private ObservableCollection<StudyPlan> recentStudyPlans = [];
    #endregion
    #endregion

    #region Constructors
    public HomePageViewModel()
    {
        Utility.Utility.AssertDesignMode();

        databaseService = new DatabaseService();
    }

    public HomePageViewModel(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
    }
    #endregion

    #region Updates
    public void UpdateItems()
    {
        UniversityCount = databaseService.Universities.Count();
        CourseCount = databaseService.Courses.Count();
        EquivalencyCount = databaseService.Equivalencies.Count();
        StudentCount = databaseService.Students.Count();
        StudyPlanCount = databaseService.StudyPlans.Count();

        RecentStudyPlans.Clear();
        RecentStudyPlans.AddRange(
            databaseService.StudyPlans.OrderByDescending(studyPlan => studyPlan.UpdatedAt).Take(5)
                .Include(studyPlan => studyPlan.Student).ThenInclude(student => student.University)
                .Include(studyPlan => studyPlan.DestinationUniversity));
    }
    #endregion
}
