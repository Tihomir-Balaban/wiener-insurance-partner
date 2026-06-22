using System.Reflection;
using DbUp;

namespace InsurancePartners.Web.Database;

public static class DatabaseMigrator
{
    public static void ApplyDatabaseMigrations(this WebApplication app)
    {
        var connectionString = app.Configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Database connection string 'DefaultConnection' is missing.");
        }

        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                Assembly.GetExecutingAssembly(),
                scriptName =>
                    scriptName.Contains(".Database.Migrations.") &&
                    scriptName.EndsWith(".sql"))
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            throw new InvalidOperationException(
                "Database migration failed.",
                result.Error);
        }
    }
}