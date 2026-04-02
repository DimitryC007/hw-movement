using Application.Common;
using Application.Data;
using Domain;
using Infrastructure.Caching.Memory;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        // Register database migration as a hosted service
        services.AddHostedService<DatabaseMigrationService>();

        services.AddScoped<DataRepository>();

        services.AddScoped<IDataRepository>(sp =>
        {
            var repository = sp.GetRequiredService<DataRepository>();
            var memoryCache = sp.GetRequiredService<IMemoryCacheService<string, DataItem>>();
            var distributedCache = sp.GetRequiredService<IDistributedCacheService>();

            return new CachedDataRepository(repository, memoryCache, distributedCache);
        });

        return services;
    }
}
