using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using CourseEquivalencyDesktop.ViewModels.Equivalencies;
using CourseEquivalencyDesktop.ViewModels.General;
using Microsoft.EntityFrameworkCore;

namespace CourseEquivalencyDesktop.ViewModels.Courses;

public partial class CoursesPageViewModel : BasePageViewModel<Course>
{
    #region Fields
    private WindowNotificationManager? windowNotificationManager;

    public readonly Interaction<Course?, Course?> CreateEquivalencyInteraction = new();
    #endregion

    #region Properties
    #region Observable Properties
    [ObservableProperty]
    private Course? equivalentCourse;
    #endregion
    #endregion

    #region Constants
    private const string CREATE_EQUIVALENCY_SUCCESSFUL_NOTIFICATION_TITLE = "Equivalency Created";
    private const string CREATE_EQUIVALENCY_SUCCESSFUL_NOTIFICATION_BODY_TEMPLATE = "\"{0}\" was created.";

    private const string CREATE_EQUIVALENCY_FAILED_NOTIFICATION_TITLE = "Equivalency Not Created";

    private const string CREATE_EQUIVALENCY_FAILED_NOTIFICATION_BODY_TEMPLATE =
        "An error occurred and \"{0}\" could not be created.";

    private const string DELETE_EQUIVALENCY_SUCCESSFUL_NOTIFICATION_TITLE = "Equivalency Deleted";
    private const string DELETE_EQUIVALENCY_SUCCESSFUL_NOTIFICATION_BODY_TEMPLATE = "\"{0}\" was deleted.";

    private const string DELETE_EQUIVALENCY_FAILED_NOTIFICATION_TITLE = "Equivalency Not Deleted";

    private const string DELETE_EQUIVALENCY_FAILED_NOTIFICATION_BODY_TEMPLATE =
        "An error occurred and \"{0}\" could not be deleted.";

    private const string COURSE_DELETE_BODY =
        "Are you sure you wish to delete \"{0}\"?\nThis action cannot be undone and will delete all associated entries.";
    #endregion

    #region Constructors
    public CoursesPageViewModel()
    {
    }

    public CoursesPageViewModel(Course? equivalentCourse, DatabaseService databaseService,
        UserSettingsService userSettingsService, GenericDialogService genericDialogService,
        ToastNotificationService toastNotificationService) : base(databaseService, userSettingsService,
        genericDialogService, toastNotificationService)
    {
        EquivalentCourse = equivalentCourse;
    }
    #endregion

    #region BasePageView
    protected override string ObjectTypeName => "Course";

    public override void UpdateItems()
    {
        Items.Clear();

        // Using the Include to eagerly load the universities so they are ready on first page view
        Items.AddRange(DatabaseService.Courses.Include(course => course.University)
            .Include(course => course.Equivalencies));
    }

    protected override Task<HashSet<Course>> Remove(Course item)
    {
        DatabaseService.Courses.Remove(item);
        return Task.FromResult<HashSet<Course>>([item]);
    }

    protected override string GetDeleteBody(Course item)
    {
        return string.Format(COURSE_DELETE_BODY, item.Name);
    }

    protected override string GetName(Course? item)
    {
        return item?.Name ?? ObjectTypeName;
    }

    protected override bool Filter(object arg)
    {
        if (arg is not Course course)
        {
            return false;
        }

        return (EquivalentCourse is null ||
                EquivalentCourse.Id != course.Id) &&
               (string.IsNullOrWhiteSpace(SearchText) ||
                course.Name.CaseInsensitiveContains(SearchText) ||
                course.University.Name.CaseInsensitiveContains(SearchText) ||
                course.CourseId.CaseInsensitiveContains(SearchText));
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

    #region Commands
    [RelayCommand]
    private async Task CreateEquivalency(Course course)
    {
        if (EquivalentCourse is null)
        {
            await CreateEquivalencyInteraction.HandleAsync(course);
            ItemsCollectionView.Refresh(); // TODO: Check if this is necessary
            return;
        }

        course.Equivalencies.Add(EquivalentCourse);
        EquivalentCourse.Equivalencies.Add(course);

        await DatabaseService.SaveChangesAsyncWithCallbacks(SaveChangesSuccessHandler, SaveChangesFailedHandler);

        void SaveChangesSuccessHandler()
        {
            // Force the binding to refresh since it is bound to the ID
            course.OnPropertyChanged(nameof(course.Id));

            ToastNotificationService.ShowToastNotification(windowNotificationManager,
                CREATE_EQUIVALENCY_SUCCESSFUL_NOTIFICATION_TITLE,
                string.Format(CREATE_EQUIVALENCY_SUCCESSFUL_NOTIFICATION_BODY_TEMPLATE,
                    EquivalenciesPageViewModel.GetEquivalencyName(course, EquivalentCourse)), NotificationType.Success);
        }

        void SaveChangesFailedHandler()
        {
            ToastNotificationService.ShowToastNotification(windowNotificationManager,
                CREATE_EQUIVALENCY_FAILED_NOTIFICATION_TITLE,
                string.Format(CREATE_EQUIVALENCY_FAILED_NOTIFICATION_BODY_TEMPLATE,
                    EquivalenciesPageViewModel.GetEquivalencyName(course, EquivalentCourse)), NotificationType.Error);
        }
    }

    [RelayCommand]
    private async Task DeleteEquivalency(Course course)
    {
        if (EquivalentCourse is null)
        {
            throw new UnreachableException("Should not be able to delete an equivalency without an equivalent course.");
        }

        course.Equivalencies.Remove(EquivalentCourse);
        EquivalentCourse.Equivalencies.Remove(course);

        await DatabaseService.SaveChangesAsyncWithCallbacks(SaveChangesSuccessHandler, SaveChangesFailedHandler);

        void SaveChangesSuccessHandler()
        {
            // Force the binding to refresh since it is bound to the ID
            course.OnPropertyChanged(nameof(course.Id));

            ToastNotificationService.ShowToastNotification(windowNotificationManager,
                DELETE_EQUIVALENCY_SUCCESSFUL_NOTIFICATION_TITLE,
                string.Format(DELETE_EQUIVALENCY_SUCCESSFUL_NOTIFICATION_BODY_TEMPLATE,
                    EquivalenciesPageViewModel.GetEquivalencyName(course, EquivalentCourse)), NotificationType.Success);
        }

        void SaveChangesFailedHandler()
        {
            ToastNotificationService.ShowToastNotification(windowNotificationManager,
                DELETE_EQUIVALENCY_FAILED_NOTIFICATION_TITLE,
                string.Format(DELETE_EQUIVALENCY_FAILED_NOTIFICATION_BODY_TEMPLATE,
                    EquivalenciesPageViewModel.GetEquivalencyName(course, EquivalentCourse)), NotificationType.Error);
        }
    }

    [RelayCommand]
    private void SetUpEquivalencyNotifications(WindowNotificationManager? notificationManager)
    {
        windowNotificationManager = notificationManager;
    }
    #endregion
}
