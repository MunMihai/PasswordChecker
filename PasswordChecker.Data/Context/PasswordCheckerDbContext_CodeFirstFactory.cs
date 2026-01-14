using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PasswordChecker.Data.Context;

public class PasswordCheckerDbContext_CodeFirstFactory : IDesignTimeDbContextFactory<PasswordCheckerDbContext_CodeFirst>
{
    public PasswordCheckerDbContext_CodeFirst CreateDbContext(string[] args)
    {
        // Build configuration from the startup project's appsettings.json
        // Try multiple possible paths to find the MVC project's appsettings.json
        var currentDir = Directory.GetCurrentDirectory();
        var possiblePaths = new[]
        {
            Path.Combine(currentDir, "..", "PasswordChecker.MVC"),
            Path.Combine(currentDir, "..", "..", "PasswordChecker.MVC"),
            Path.Combine(currentDir, "PasswordChecker.MVC"),
        };

        string? basePath = null;
        foreach (var path in possiblePaths)
        {
            var appsettingsPath = Path.Combine(path, "appsettings.json");
            if (File.Exists(appsettingsPath))
            {
                basePath = path;
                break;
            }
        }

        if (basePath == null)
        {
            throw new InvalidOperationException(
                "Could not find appsettings.json. Please ensure the PasswordChecker.MVC project exists and contains appsettings.json.");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found in appsettings.json.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<PasswordCheckerDbContext_CodeFirst>();
        optionsBuilder.UseSqlServer(
            connectionString,
            sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
        );

        return new PasswordCheckerDbContext_CodeFirst(optionsBuilder.Options);
    }
}
