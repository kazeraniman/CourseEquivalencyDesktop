using Avalonia.Controls.Notifications;
using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.ViewModels.Courses;
using CourseEquivalencyDesktop.ViewModels.DatabaseSelectionWizard;
using CourseEquivalencyDesktop.ViewModels.Equivalencies;
using CourseEquivalencyDesktop.ViewModels.Home;
using CourseEquivalencyDesktop.ViewModels.Settings;
using CourseEquivalencyDesktop.ViewModels.Students;
using CourseEquivalencyDesktop.ViewModels.StudyPlans;
using CourseEquivalencyDesktop.ViewModels.Universities;
using Microsoft.Extensions.DependencyInjection;

namespace CourseEquivalencyDesktop.Services;

/// <summary>
///     Collects all the services to produce (and any associated factories) to simplify dependency injection
///     initialization.
/// </summary>
public static class ServiceCollectionExtensions
{
    #region Factories
    public delegate CoursesPageViewModel CoursesPageViewModelFactory(Course? equivalentCourse);

    public delegate CreateOrEditUniversityViewModel CreateOrEditUniversityViewModelFactory(University? university);

    public delegate CreateOrEditCourseViewModel CreateOrEditCourseViewModelFactory(Course? course);

    public delegate CreateOrEditStudentViewModel CreateOrEditStudentViewModelFactory(Student? student);

    public delegate CreateStudyPlanViewModel CreateStudyPlanViewModelFactory(StudyPlan? studyPlan);

    public delegate EditStudyPlanViewModel EditStudyPlanViewModelFactory(StudyPlan? studyPlan);
    #endregion

    #region Services
    public static void AddCommonServices(this IServiceCollection collection,
        WindowNotificationManager windowNotificationManager)
    {
        collection.AddSingleton<FileDialogService>();
        collection.AddSingleton<UserSettingsService>();
        collection.AddSingleton<DatabaseService>();
        collection.AddSingleton<GenericDialogService>();
        collection.AddSingleton<ToastNotificationService>(_ => new ToastNotificationService(windowNotificationManager));
        collection.AddTransient<DatabaseSelectionWizardViewModel>();
        collection.AddTransient<HomePageViewModel>();
        collection.AddTransient<UniversitiesPageViewModel>();
        collection.AddTransient<StudentsPageViewModel>();
        collection.AddTransient<EquivalenciesPageViewModel>();
        collection.AddTransient<StudyPlansPageViewModel>();
        collection.AddTransient<SettingsPageViewModel>();
        collection.AddTransient<CoursesPageViewModelFactory>(provider => equivalentCourse =>
        {
            var databaseService = provider.GetRequiredService<DatabaseService>();
            var userSettingsService = provider.GetRequiredService<UserSettingsService>();
            var genericDialogService = provider.GetRequiredService<GenericDialogService>();
            var toastNotificationService = provider.GetRequiredService<ToastNotificationService>();
            return new CoursesPageViewModel(equivalentCourse, databaseService, userSettingsService,
                genericDialogService, toastNotificationService);
        });
        collection.AddTransient<CreateOrEditUniversityViewModelFactory>(provider => university =>
        {
            var databaseService = provider.GetRequiredService<DatabaseService>();
            return new CreateOrEditUniversityViewModel(university, databaseService);
        });
        collection.AddTransient<CreateOrEditCourseViewModelFactory>(provider => course =>
        {
            var databaseService = provider.GetRequiredService<DatabaseService>();
            return new CreateOrEditCourseViewModel(course, databaseService);
        });
        collection.AddTransient<CreateOrEditStudentViewModelFactory>(provider => student =>
        {
            var databaseService = provider.GetRequiredService<DatabaseService>();
            return new CreateOrEditStudentViewModel(student, databaseService);
        });
        collection.AddTransient<CreateStudyPlanViewModelFactory>(provider => studyPlan =>
        {
            var databaseService = provider.GetRequiredService<DatabaseService>();
            return new CreateStudyPlanViewModel(studyPlan, databaseService);
        });
        collection.AddTransient<EditStudyPlanViewModelFactory>(provider => studyPlan =>
        {
            var fileDialogService = provider.GetRequiredService<FileDialogService>();
            var userSettingsService = provider.GetRequiredService<UserSettingsService>();
            var databaseService = provider.GetRequiredService<DatabaseService>();
            return new EditStudyPlanViewModel(studyPlan, fileDialogService, userSettingsService, databaseService);
        });
    }
    #endregion
}
