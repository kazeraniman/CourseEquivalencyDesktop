using Microsoft.EntityFrameworkCore.Design;

namespace CourseEquivalencyDesktop.Services;

/// <summary>
///     Creates a <see cref="DatabaseService" /> for design time to ensure issues to not arise in the designer.
/// </summary>
public class DatabaseServiceDesignTimeFactory : IDesignTimeDbContextFactory<DatabaseService>
{
    #region Factory
    public DatabaseService CreateDbContext(string[] args)
    {
        return new DatabaseService(true);
    }
    #endregion
}
