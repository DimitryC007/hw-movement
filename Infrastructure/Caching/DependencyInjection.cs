using Application.Common;
using Infrastructure.Caching.Memory;
using Infrastructure.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Caching;

public static class DependencyInjection
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MemoryCacheOptions>()
            .Bind(configuration.GetSection(MemoryCacheOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(typeof(IMemoryCacheService<,>), typeof(MemoryCacheService<,>));

        services.AddOptions<RedisCacheOptions>()
            .Bind(configuration.GetSection(RedisCacheOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddStackExchangeRedisCache(opts =>
        {
            opts.Configuration = configuration.GetConnectionString("Redis");
            opts.InstanceName = "app:";
        });

        services.AddSingleton<IDistributedCacheService, RedisCacheService>();

        return services;
    }
}
