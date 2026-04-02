namespace Infrastructure.Caching.Memory;

public interface IMemoryCacheService<TKey, TValue>
{
    void Set(TKey key, TValue value);
    TValue? Get(TKey key);
}
