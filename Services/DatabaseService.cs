using CommunityToolkit.Mvvm.DependencyInjection;
using CourseEquivalencyDesktop.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseEquivalencyDesktop.Services;

public class DatabaseService : DbContext
{
    private const string DESIGN_TIME_DB = "test.db";

    private readonly bool isDesignTime;

    public DatabaseService() { }

    public DatabaseService(bool isDesignTime)
    {
        this.isDesignTime = isDesignTime;
    }

    public DbSet<University> Universities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={(isDesignTime ? DESIGN_TIME_DB : Ioc.Default.GetRequiredService<UserSettingsService>().DatabaseFilePath)}");
        base.OnConfiguring(optionsBuilder);
    }
}
