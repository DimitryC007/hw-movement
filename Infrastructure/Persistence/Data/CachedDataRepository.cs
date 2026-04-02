using Application.Common;
using Application.Data;
using Domain;
using Infrastructure.Caching.Memory;

namespace Infrastructure.Persistence.Data;

public class CachedDataRepository : IDataRepository
{
    private readonly IDataRepository _dataRepository;
    private readonly IMemoryCacheService<string, DataItem> _memoryCache;
    private readonly IDistributedCacheService _distributedCache;

    public CachedDataRepository(
        IDataRepository dataRepository,
        IMemoryCacheService<string, DataItem> memoryCache,
        IDistributedCacheService distributedCache)
    {
        _dataRepository = dataRepository;
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
    }

    public Task<DataItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return GetOrLoadAsync(id, cancellationToken);
    }

    public Task<DataItem> AddAsync(DataItem data, CancellationToken cancellationToken)
    {
        return _dataRepository.AddAsync(data, cancellationToken);
    }

    private async Task<DataItem?> GetOrLoadAsync(Guid id, CancellationToken cancellationToken)
    {
        var dataId = id.ToString();

        // Try distributed cache
        DataItem? data = await _distributedCache.GetAsync<DataItem>(dataId);
        if (data is not null)
        {
            return data;
        }

        // Try memory cache
        data = _memoryCache.Get(dataId);
        if (data is not null)
        {
            await _distributedCache.SetAsync(dataId, data);
            return data;
        }

        // Load from DB
        data = await _dataRepository.GetByIdAsync(id, cancellationToken);
        if (data is null)
        {
            return null;
        }

        // Fill both caches
        _memoryCache.Set(dataId, data);
        await _distributedCache.SetAsync(dataId, data);

        return data;
    }
}
