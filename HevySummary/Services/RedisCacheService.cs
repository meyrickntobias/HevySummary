using System.Text.Json;
using StackExchange.Redis;

namespace HevySummary.Services;

public class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
{
    public async Task<T?> GetCacheValueAsync<T>(string key)
    {
        var db = redis.GetDatabase();
        var serializedValue = await db.StringGetAsync(key);
        return serializedValue.IsNullOrEmpty 
            ? default 
            : JsonSerializer.Deserialize<T>(serializedValue.ToString());
    }

    public async Task<T> SetCacheValueAsync<T>(
        string key,
        T value,
        TimeSpan? absoluteExpirationRelativeToNow = null)
    {
        var db = redis.GetDatabase();
        var serializedValue = JsonSerializer.Serialize(value);
        await db.StringSetAsync(key, serializedValue, absoluteExpirationRelativeToNow);
        return value;
    }
}