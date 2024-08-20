using Microsoft.EntityFrameworkCore.Design;

namespace CourseEquivalencyDesktop.Services;

public class DatabaseServiceDesignTimeFactory : IDesignTimeDbContextFactory<DatabaseService>
{
    public DatabaseService CreateDbContext(string[] args)
    {
        return new DatabaseService(true);
    }
}
