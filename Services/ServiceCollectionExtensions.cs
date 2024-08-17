using CourseEquivalencyDesktop.ViewModels.DatabaseSelectionWizard;
using Microsoft.Extensions.DependencyInjection;

namespace CourseEquivalencyDesktop.Services;

public static class ServiceCollectionExtensions {
    public static void AddCommonServices(this IServiceCollection collection) {
        collection.AddSingleton<FileDialogService>();
        collection.AddTransient<DatabaseSelectionWizardViewModel>();
    }
}
