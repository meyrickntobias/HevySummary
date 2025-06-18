using StackExchange.Redis;

namespace HevySummary.Services;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }
    
    public async Task<string?> GetCacheValue(string key)
    {
        var db = _redis.GetDatabase();
        return await db.StringGetAsync(key);
    }

    public Task SetCacheValue(string key, string value)
    {
        var db = _redis.GetDatabase();
        return db.StringSetAsync(key, value);
    }
}