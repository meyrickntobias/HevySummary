namespace HevySummary.Services;

public interface ICacheService
{
    public Task<string?> GetCacheValue(string key);
    
    public Task SetCacheValue(string key, string value);
}