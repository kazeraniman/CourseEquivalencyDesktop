using CourseEquivalencyDesktop.Models;
using CourseEquivalencyDesktop.ViewModels.DatabaseSelectionWizard;
using CourseEquivalencyDesktop.ViewModels.Universities;
using Microsoft.Extensions.DependencyInjection;

namespace CourseEquivalencyDesktop.Services;

public static class ServiceCollectionExtensions {
    public delegate CreateOrEditUniversityViewModel CreateOrEditUniversityViewModelFactory(University? university);

    public static void AddCommonServices(this IServiceCollection collection) {
        collection.AddSingleton<FileDialogService>();
        collection.AddSingleton<UserSettingsService>();
        collection.AddSingleton<DatabaseService>();
        collection.AddTransient<DatabaseSelectionWizardViewModel>();
        collection.AddTransient<UniversitiesPageViewModel>();
        collection.AddTransient<CreateOrEditUniversityViewModelFactory>(provider => university =>
        {
            var databaseService = provider.GetRequiredService<DatabaseService>();
            return new CreateOrEditUniversityViewModel(university, databaseService);
        });
    }
}
