using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.ViewModels.Courses;
using CourseEquivalencyDesktop.ViewModels.DatabaseSelectionWizard;
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
    public delegate CreateOrEditUniversityViewModel CreateOrEditUniversityViewModelFactory(University? university);

    public delegate CreateOrEditCourseViewModel CreateOrEditCourseViewModelFactory(Course? course);
    #endregion

    #region Services
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<FileDialogService>();
        collection.AddSingleton<UserSettingsService>();
        collection.AddSingleton<DatabaseService>();
        collection.AddSingleton<GenericDialogService>();
        collection.AddTransient<DatabaseSelectionWizardViewModel>();
        collection.AddTransient<UniversitiesPageViewModel>();
        collection.AddTransient<CoursesPageViewModel>();
        collection.AddTransient<CreateOrEditUniversityViewModelFactory>(provider => university =>
        {
            var databaseService = provider.GetRequiredService<DatabaseService>();
            var genericDialogService = provider.GetRequiredService<GenericDialogService>();
            return new CreateOrEditUniversityViewModel(university, databaseService, genericDialogService);
        });
        collection.AddTransient<CreateOrEditCourseViewModelFactory>(provider => course =>
        {
            var databaseService = provider.GetRequiredService<DatabaseService>();
            var genericDialogService = provider.GetRequiredService<GenericDialogService>();
            return new CreateOrEditCourseViewModel(course, databaseService, genericDialogService);
        });
    }
    #endregion
}
