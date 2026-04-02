using Application.Common;
using Application.Users;
using Domain.Users;
using Infrastructure.Caching.Memory;

namespace Infrastructure.Persistence.Users;

public class CachedUserRepository : IUserRepository
{
    private readonly IUserRepository _userRepository;
    private readonly IMemoryCacheService<string, User> _memoryCache;
    private readonly IDistributedCacheService _distributedCache;

    public CachedUserRepository(
        IUserRepository userRepository,
        IMemoryCacheService<string, User> memoryCache,
        IDistributedCacheService distributedCache)
    {
        _userRepository = userRepository;
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return GetOrLoadAsync(id, cancellationToken);
    }

    public Task<User> AddAsync(User user, CancellationToken cancellationToken)
    {
        return _userRepository.AddAsync(user, cancellationToken);
    }
    private async Task<User?> GetOrLoadAsync(Guid id, CancellationToken cancellationToken)
    {
        var userId = id.ToString();

        // Try distributed cache
        User? user = await _distributedCache.GetAsync<User>(userId);
        if (user is not null)
        {
            return user;
        }

        // Try memory cache
        user = _memoryCache.Get(userId);
        if (user is not null)
        {
            await _distributedCache.SetAsync(userId, user);
            return user;
        }

        // Load from DB
        user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return null;
        }

        // Fill both caches
        _memoryCache.Set(userId, user);
        await _distributedCache.SetAsync(userId, user);

        return user;
    }
}
