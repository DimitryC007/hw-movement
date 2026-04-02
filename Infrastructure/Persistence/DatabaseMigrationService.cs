using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public class DatabaseMigrationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseMigrationService> _logger;

    public DatabaseMigrationService(
        IServiceProvider serviceProvider,
        ILogger<DatabaseMigrationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DatabaseMigrationService: Starting database migration...");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            _logger.LogInformation("DatabaseMigrationService: Checking for pending migrations...");

            var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
            var pendingCount = pendingMigrations.Count();

            _logger.LogInformation("*** DatabaseMigrationService: Found {pendingCount} pending migrations ***", pendingCount);

            if (pendingCount > 0)
            {
                _logger.LogInformation("DatabaseMigrationService: Found {Count} pending migration(s). Applying...", pendingCount);
                await context.Database.MigrateAsync(cancellationToken);
                _logger.LogInformation("DatabaseMigrationService: Database migration completed successfully!");
            }
            else
            {
                _logger.LogInformation("DatabaseMigrationService: Database is already up to date. No migrations needed.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DatabaseMigrationService: An error occurred while migrating the database: {Message}", ex.Message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DatabaseMigrationService: Stopped");
        return Task.CompletedTask;
    }
}
