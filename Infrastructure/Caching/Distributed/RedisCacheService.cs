using Application.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Infrastructure.Caching.Distributed;

internal class RedisCacheService : IDistributedCacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly RedisCacheOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IDistributedCache distributedCache, IOptions<RedisCacheOptions> options)
    {
        _distributedCache = distributedCache;
        _options = options.Value;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var ttl = expiry ?? TimeSpan.FromSeconds(_options.DefaultTTLSeconds);
        var serialized = JsonSerializer.Serialize(value, _jsonOptions);
        var entryOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl };
        
        await _distributedCache.SetStringAsync(key, serialized, entryOptions);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _distributedCache.GetStringAsync(key);
        
        if (value is null)
            return default;

        return JsonSerializer.Deserialize<T>(value, _jsonOptions);
    }
}
