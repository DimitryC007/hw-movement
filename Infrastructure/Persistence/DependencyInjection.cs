using Application.Common;
using Application.Users;
using Domain.Users;
using Infrastructure.Caching.Memory;
using Infrastructure.Persistence.Users;
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

        services.AddScoped<UserRepository>();

        services.AddScoped<IUserRepository>(sp =>
        {
            var repository = sp.GetRequiredService<UserRepository>();
            var memoryCache = sp.GetRequiredService<IMemoryCacheService<string, User>>();
            var distributedCache = sp.GetRequiredService<IDistributedCacheService>();

            return new CachedUserRepository(repository, memoryCache, distributedCache);
        });



        return services;
    }
}
