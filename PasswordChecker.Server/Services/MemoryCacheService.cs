using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PasswordChecker.Server.Services.Interfaces;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace PasswordChecker.Server.Services;

public class MemoryCacheService : IMemoryCacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryCacheService> _logger;
    private readonly ConcurrentDictionary<string, object> _keys = new();

    public MemoryCacheService(
        IMemoryCache cache,
        ILogger<MemoryCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null) where T : class
    {
        if (_cache.TryGetValue(key, out T? cachedValue) && cachedValue != null)
        {
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return cachedValue;
        }

        _logger.LogDebug("Cache miss for key: {Key}, fetching from factory", key);
        var value = await factory();

        if (value != null)
        {
            Set(key, value, expiration);
        }

        return value;
    }

    public T? Get<T>(string key) where T : class
    {
        if (_cache.TryGetValue(key, out T? value))
        {
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return value;
        }

        _logger.LogDebug("Cache miss for key: {Key}", key);
        return null;
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        };

        _cache.Set(key, value, options);
        _keys.TryAdd(key, null);
        _logger.LogDebug("Cached value for key: {Key} with expiration: {Expiration}", key, expiration?.TotalMinutes ?? 5);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        _keys.TryRemove(key, out _);
        _logger.LogDebug("Removed cache entry for key: {Key}", key);
    }

    public void RemoveByPattern(string pattern)
    {
        var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        var keysToRemove = _keys.Keys.Where(key => regex.IsMatch(key)).ToList();

        foreach (var key in keysToRemove)
        {
            Remove(key);
        }

        _logger.LogInformation("Removed {Count} cache entries matching pattern: {Pattern}", keysToRemove.Count, pattern);
    }

    public void Clear()
    {
        var keysToRemove = _keys.Keys.ToList();
        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
        }
        _keys.Clear();
        _logger.LogInformation("Cleared all cache entries ({Count} entries)", keysToRemove.Count);
    }
}
