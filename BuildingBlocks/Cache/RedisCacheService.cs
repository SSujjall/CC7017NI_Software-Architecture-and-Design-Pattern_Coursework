using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Cache;

public class RedisCacheService(
    IDistributedCache _cache,
    ILogger<RedisCacheService> _logger
) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key)
    {
        var cached = await _cache.GetStringAsync(key);
        if (cached is null)
        {
            _logger.LogInformation("Cache MISS: {Key}", key);
            return default;
        }
        _logger.LogInformation("Cache HIT: {Key}", key);
        return JsonSerializer.Deserialize<T>(cached);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiry)
    {
        var serialized = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, serialized, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        });
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}