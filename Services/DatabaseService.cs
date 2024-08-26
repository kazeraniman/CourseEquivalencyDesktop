using CommunityToolkit.Mvvm.DependencyInjection;
using CourseEquivalencyDesktop.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseEquivalencyDesktop.Services;

/// <summary>
///     Allows access to the database.
/// </summary>
public class DatabaseService : DbContext
{
    #region Constants
    private const string DESIGN_TIME_DB = "test.db";
    #endregion

    #region Fields
    private readonly bool isDesignTime;
    #endregion

    #region Properties
    public DbSet<University> Universities { get; set; }
    #endregion

    #region Constructors
    public DatabaseService()
    {
    }

    public DatabaseService(bool isDesignTime)
    {
        this.isDesignTime = isDesignTime;
    }
    #endregion

    #region DbContext
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(
            $"Data Source={(isDesignTime ? DESIGN_TIME_DB : Ioc.Default.GetRequiredService<UserSettingsService>().DatabaseFilePath)}");
        base.OnConfiguring(optionsBuilder);
    }
    #endregion
}
