using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CourseEquivalencyDesktop.Services;
using CourseEquivalencyDesktop.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CourseEquivalencyDesktop.ViewModels.General;

public partial class MainWindowViewModel : BaseViewModel
{
    #region Fields
    public readonly Interaction<bool?, bool?> SpawnDatabaseSelectionWizardInteraction = new();
    #endregion

    #region Properties
    #region Observable Properties
    [ObservableProperty] private BaseViewModel currentContent = new MainPageLoadingViewModel();
    #endregion
    #endregion

    #region Commands
    [RelayCommand]
    private async Task Initialization()
    {
        // Initial delay so that nothing starts until after the window is fully opened
        await Task.Delay(1000);

        // Register all the services needed for the application to run
        var collection = new ServiceCollection();
        collection.AddCommonServices();

        // Creates a ServiceProvider containing services from the provided IServiceCollection
        var services = collection.BuildServiceProvider();

        // Make the services generally available
        Ioc.Default.ConfigureServices(services);

        // Load the settings file
        var userSettingsService = Ioc.Default.GetRequiredService<UserSettingsService>();
        await userSettingsService.LoadSettings();

        // Show the database selection wizard if we don't have a stored database yet
        if (string.IsNullOrEmpty(Ioc.Default.GetRequiredService<UserSettingsService>().DatabaseFilePath))
        {
            await SpawnDatabaseSelectionWizardInteraction.HandleAsync(null);
        }

        // Create the database if needed and apply any necessary migrations
        await Ioc.Default.GetRequiredService<DatabaseService>().Database.MigrateAsync();

        // Show the actual content now that we are ready
        CurrentContent = new MainPageViewModel();
    }
    #endregion
}
