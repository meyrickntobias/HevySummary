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

    public async Task AddToHashSetAsync<T>(string key, string hashKey, T value)
    {
        var db = redis.GetDatabase();
        var serializedValue = JsonSerializer.Serialize(value);
        var hashEntry = new HashEntry(hashKey, serializedValue);
        await db.HashSetAsync(key, [hashEntry]);
    }
    
    public async Task AddToHashSetAsync<T>(string key, IEnumerable<T> values, Func<T, string> hashKeySelector)
    {
        var db = redis.GetDatabase();
        var hashEntries = values.Select(value => new HashEntry(hashKeySelector(value), JsonSerializer.Serialize(value)));
        await db.HashSetAsync(key, hashEntries.ToArray());
    }

    public async Task RemoveFromHashSetAsync(string key, string hashKey)
    {
        var db = redis.GetDatabase();
        await db.HashDeleteAsync(key, hashKey);
    }

    public async Task<List<T>> GetAllFromHashSetAsync<T>(string key)
    {
        var db = redis.GetDatabase();
        var hashEntries = await db.HashGetAllAsync(key);
        var result = new List<T>();
        foreach (var hashEntry in hashEntries)
        {
            if (hashEntry.Value.IsNullOrEmpty) continue;
            var deserializedEntity = JsonSerializer.Deserialize<T>(hashEntry.Value.ToString());
            if (deserializedEntity != null)
            {
                result.Add(deserializedEntity);
            }
        }
        return result;
    }
}