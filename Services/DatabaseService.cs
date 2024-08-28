using System;
using System.Threading.Tasks;
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
    public DbSet<Course> Courses { get; set; }
    #endregion

    #region Events
    public event Action? OnSaveChangesSucceeded;
    public event Action? OnSaveChangesFailed;
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

    #region Handlers
    private void SaveChangesSuccessHandler(object? _0, SavedChangesEventArgs _1)
    {
        OnSaveChangesSucceeded?.Invoke();
    }

    private void SaveChangedFailedHandler(object? _0, SaveChangesFailedEventArgs _1)
    {
        OnSaveChangesFailed?.Invoke();
    }
    #endregion

    #region DbContext
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(
            $"Data Source={(isDesignTime ? DESIGN_TIME_DB : Ioc.Default.GetRequiredService<UserSettingsService>().DatabaseFilePath)}");

        if (!isDesignTime)
        {
            SaveChangesFailed += SaveChangedFailedHandler;
            SavedChanges += SaveChangesSuccessHandler;
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<University>()
            .HasMany(e => e.Courses)
            .WithOne(e => e.University)
            .HasForeignKey(e => e.UniversityId)
            .HasPrincipalKey(e => e.Id);
    }
    #endregion

    #region Additional Functionality
    public async Task SaveChangesAsyncWithCallbacks(Action? onSaveChangesSucceeded = null,
        Action? onSaveChangesFailed = null)
    {
        OnSaveChangesSucceeded += onSaveChangesSucceeded;
        OnSaveChangesFailed += onSaveChangesFailed;

        await SaveChangesAsync();

        OnSaveChangesSucceeded -= onSaveChangesSucceeded;
        OnSaveChangesFailed -= onSaveChangesFailed;
    }
    #endregion
}
