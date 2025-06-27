namespace HevySummary.Services;

public interface ICacheService
{
    public Task<T?> GetCacheValueAsync<T>(string key);
    
    public Task<T> SetCacheValueAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null);
}