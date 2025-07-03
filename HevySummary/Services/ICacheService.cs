namespace HevySummary.Services;

public interface ICacheService
{
    public Task<T?> GetCacheValueAsync<T>(string key);
    
    public Task<T> SetCacheValueAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null);

    public Task AddToHashSetAsync<T>(string key, string hashKey, T value);

    public Task AddToHashSetAsync<T>(string key, IEnumerable<T> values, Func<T, string> hashKeySelector);
    
    public Task RemoveFromHashSetAsync(string key, string hashKey);

    public Task<List<T>> GetAllFromHashSetAsync<T>(string key);
}